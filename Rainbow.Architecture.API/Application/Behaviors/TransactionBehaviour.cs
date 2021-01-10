using MediatR;
using Microsoft.Extensions.Logging;
using Rainbow.Architecture.API.Application.IntegrationEvents;
using Rainbow.Architecture.Infrastructure;
using Rainbow.Architecture.Infrastructure.Extensions;
using Serilog.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Application.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly AppDbContext _dbContext;
        private readonly IAppIntegrationEventService _appIntegrationEventService;

        public TransactionBehaviour(AppDbContext dbContext,
            IAppIntegrationEventService orderingIntegrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentException(nameof(AppDbContext));
            _appIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentException(nameof(orderingIntegrationEventService));
            _logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();

            try
            {
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }

                Guid transactionId;

                // 
                using (_dbContext)
                {
                    _dbContext.BeginTransaction();
                    transactionId = _dbContext.TransactionId;

                    using (LogContext.PushProperty("TransactionContext", _dbContext.TransactionId))
                    {
                        _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", _dbContext.TransactionId, typeName, request);

                        response = await next();

                        _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", _dbContext.TransactionId, typeName);

                        await _dbContext.SaveEntitiesAsync();
                    }
                }

                await _appIntegrationEventService.PublishEventsThroughEventBusAsync(transactionId);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
