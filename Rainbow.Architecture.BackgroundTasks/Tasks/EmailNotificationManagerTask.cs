using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rainbow.Architecture.BackgroundTasks.Events;
using Rainbow.Extensions.EventBus.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rainbow.Architecture.BackgroundTasks.Tasks
{
    public class EmailNotificationService : BackgroundService
    {
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly BackgroundTaskSettings _settings;
        private readonly IEventBus _eventBus;

        public EmailNotificationService(IOptions<BackgroundTaskSettings> settings, IEventBus eventBus, ILogger<EmailNotificationService> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("EmailNotificationService is starting.");

            stoppingToken.Register(() => _logger.LogDebug("#1 EmailNotificationService background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug("EmailNotificationService background task is doing background work.");

                CheckConfirmedGracePeriodItems();

                await Task.Delay(_settings.CheckUpdateTime, stoppingToken);
            }

            _logger.LogDebug("EmailNotificationService background task is stopping.");

            await Task.CompletedTask;
        }

        private void CheckConfirmedGracePeriodItems()
        {
            _logger.LogDebug("Checking confirmed grace period todoItems");

            var todoIds = GetToDoItems();

            foreach (var orderId in todoIds)
            {
                var @event = new EmailNotificationIntegrationEvent(orderId);

                _logger.LogInformation("----- Publishing integration event: {IntegrationEventId} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                _eventBus.Publish(@event);
            }
        }

        private IEnumerable<Guid> GetToDoItems()
        {
            List<Guid> ids = new List<Guid>();
            //TODO
            Thread.Sleep(TimeSpan.FromSeconds(3));
            ids.Add(Guid.NewGuid());
            return ids;
        }
    }
}
