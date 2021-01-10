using Microsoft.Extensions.Logging;
using Rainbow.Architecture.API.Application.IntegrationEvents.IntegrationEventLog;
using Rainbow.Extensions.EventBus.Abstractions;
using Rainbow.Extensions.EventBus.Abstractions.Events;
using System;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Application.IntegrationEvents
{
    public class AppIntegrationEventService : IAppIntegrationEventService
    {
        private readonly IEventBus _eventBus;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ILogger<AppIntegrationEventService> _logger;

        public AppIntegrationEventService(IEventBus eventBus,
            IIntegrationEventLogService eventLogService,
            ILogger<AppIntegrationEventService> logger)
        {
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", logEvt.EventId, Program.AppName, logEvt.IntegrationEvent);

                try
                {
                    await _eventLogService.MarkEventAsInProgressAsync(logEvt.EventId);
                    _eventBus.Publish(logEvt.IntegrationEvent);
                    await _eventLogService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId} from {AppName}", logEvt.EventId, Program.AppName);

                    await _eventLogService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent evt)
        {
            _logger.LogInformation("----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

            await _eventLogService.SaveEventAsync(evt);
        }
    }
}
