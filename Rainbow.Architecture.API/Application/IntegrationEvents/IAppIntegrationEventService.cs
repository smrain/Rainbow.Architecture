using Rainbow.Extensions.EventBus.Abstractions.Events;
using System;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Application.IntegrationEvents
{
    public interface IAppIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}
