using Rainbow.Extensions.EventBus.Abstractions.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Application.IntegrationEvents.IntegrationEventLog
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
        Task SaveEventAsync(IntegrationEvent @event);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}
