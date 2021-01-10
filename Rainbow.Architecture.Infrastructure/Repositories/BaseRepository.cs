using Rainbow.Architecture.Domain.SeedWork;
using System;
using System.Threading.Tasks;

namespace Rainbow.Architecture.Infrastructure.Repositories
{
    public abstract class BaseRepository<TEntity, TKey>
        where TEntity : Entity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected readonly AppDbContext DbContext;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return DbContext;
            }
        }

        protected BaseRepository(AppDbContext context)
        {
            DbContext = context ?? throw new ArgumentNullException(nameof(context));
        }


        public virtual async Task<TEntity> GetAsync(TKey id)
        {
            return await DbContext.GetAsync<TEntity>(o => o.Id.Equals(id));
        }

        public virtual async Task<TKey> AddAsync(TEntity entity)
        {
            return await DbContext.AddAsync(entity);
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            return await DbContext.UpdateAsync(entity);
        }
    }
}
