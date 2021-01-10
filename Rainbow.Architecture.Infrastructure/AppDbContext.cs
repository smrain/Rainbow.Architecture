using MediatR;
using MySql.Data.MySqlClient;
using Rainbow.Architecture.Domain.SeedWork;
using Rainbow.DapperExtensions.Repository;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rainbow.Architecture.Infrastructure
{
    public class AppDbContext : EntityRepository, IUnitOfWork
    {
        private readonly string _connectionString;
        private static readonly SqlType[] _sqlTypes = new SqlType[] { SqlType.Unknown, SqlType.DQL };
        private readonly ConcurrentQueue<INotification> _domianEvents = new ConcurrentQueue<INotification>();
        private readonly IMediator _mediator;

        public new Guid TransactionId => base.TransactionId;

        public new bool HasActiveTransaction => base.HasActiveTransaction;

        public AppDbContext(string connectionString, IMediator mediator) : base(connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            Rainbow.DapperExtensions.DapperExtensions.SetMappingAssemblies(new Assembly[] { this.GetType().Assembly });

            System.Diagnostics.Debug.WriteLine("AppDbContext::ctor ->" + this.GetHashCode());
        }

        protected override IDbConnection GetConnection() => new MySqlConnection();


        protected override TResult Execute<TResult>(Func<TResult> fun, SqlType sqlType = SqlType.Unknown)
        {
            TResult result = default;

            if (Connection != null && fun != null)
            {
                try
                {
                    base.TryOpenConnection();

                    if (!_sqlTypes.Contains(sqlType) && this is IUnitOfWork)
                    {
                        //Begin Transaction
                        base.BeginTransaction();

                        FillDomainEvents(fun.Target);
                    }

                    result = fun.Invoke();
                }
                catch (Exception ex)
                {
                    Rollback();
                    throw ex;
                }
                finally
                {
                    if (!HasActiveTransaction && !(result is DapperExtensions.IMultipleResultReader || result is IDataReader))
                    {
                        Dispose();
                    }
                }
            }
            return result;
        }
        protected override void Execute(Action action, SqlType sqlType = SqlType.Unknown)
        {
            if (Connection != null && action != null)
            {
                try
                {
                    base.TryOpenConnection();
                    if (!_sqlTypes.Contains(sqlType) && this is IUnitOfWork)
                    {
                        base.BeginTransaction();
                    }
                    action.Invoke();
                }
                catch (Exception ex)
                {
                    Rollback();
                    throw ex;
                }
                finally
                {
                    if (!HasActiveTransaction)
                    {
                        Dispose();
                    }
                }
            }
        }

        protected override async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> fun, SqlType sqlType = SqlType.Unknown)
        {
            TResult result = default;
            if (Connection != null && fun != null)
            {
                try
                {
                    TryOpenConnection();

                    if (!_sqlTypes.Contains(sqlType) && this is IUnitOfWork)
                    {
                        //Begin Transaction
                        base.BeginTransaction();

                        FillDomainEvents(fun.Target);
                    }

                    result = await fun.Invoke();
                }
                catch (Exception ex)
                {
                    Rollback();
                    throw ex;
                }
                finally
                {
                    if (!HasActiveTransaction && !(result is DapperExtensions.IMultipleResultReader || result is IDataReader))
                    {
                        Dispose();
                    }
                }
            }
            return result;
        }

        protected override async Task ExecuteAsync(Func<Task> action, SqlType sqlType = SqlType.Unknown)
        {
            if (Connection != null && action != null)
            {
                try
                {
                    TryOpenConnection();
                    if (!_sqlTypes.Contains(sqlType) && this is IUnitOfWork)
                    {
                        base.BeginTransaction();
                    }
                    await action.Invoke();
                }
                catch (Exception ex)
                {
                    Rollback();
                    throw ex;
                }
                finally
                {
                    if (!HasActiveTransaction)
                    {
                        Dispose();
                    }
                }
            }
        }

        private void FillDomainEvents(object target)
        {
            //fill DomainEvents
            if (target != null)
            {
                foreach (var field in target.GetType().GetFields())
                {
                    var filedName = field.Name;
                    object filedValue = field.GetValue(target);

                    if (filedValue != null && filedValue is IDomainEntity domainEntity)
                    {
                        IReadOnlyCollection<INotification> domainEvents = domainEntity.DomainEvents;
                        if (domainEvents != null && domainEvents?.Count > 0)
                        {
                            domainEvents.ToList().ForEach(e => _domianEvents.Enqueue(e));
                            domainEntity.ClearDomainEvents();
                        }
                    }
                }
            }
        }

        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            if (_domianEvents?.Count > 0)
            {
                foreach (var domainEvent in _domianEvents)
                {
                    _mediator.Publish(domainEvent);
                }
                _domianEvents.Clear();
            }

            base.Commit();
            return Task.FromResult(true);
        }

        public Task<bool> SaveChangeAsync(CancellationToken cancellationToken = default)
        {
            base.Commit();
            return Task.FromResult(true);
        }


        #region Extension Method

        // 分页查询
        public (IEnumerable<T>, int) GetPage<T>(string sql, object param = null) where T : class
            => Execute(() =>
            {
                var rlt = base.QueryMultiple(sql, param);
                return (rlt.Read<T>(), rlt.ReadFirstOrDefault<int>());
            }, SqlType.DQL);

        public async Task<(IEnumerable<T>, int)> GetPageAsync<T>(string sql, object param = null) where T : class
            => await ExecuteAsync(async () =>
            {
                var rlt = await base.QueryMultipleAsync(sql, param);
                return (await rlt.ReadAsync<T>(), await rlt.ReadFirstOrDefaultAsync<int>());
            }, SqlType.DQL);

        #endregion Extension Method
    }

}
