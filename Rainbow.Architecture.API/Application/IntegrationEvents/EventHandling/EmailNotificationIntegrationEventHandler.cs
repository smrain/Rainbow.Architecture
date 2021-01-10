using MediatR;
using Microsoft.Extensions.Logging;
using Rainbow.Architecture.API.Application.IntegrationEvents.Events;
using Rainbow.Extensions.EventBus.Abstractions;
using Serilog.Context;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Application.IntegrationEvents.EventHandling
{
    public class EmailNotificationIntegrationEventHandler : IIntegrationEventHandler<EmailNotificationIntegrationEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmailNotificationIntegrationEventHandler> _logger;

        public EmailNotificationIntegrationEventHandler(
            IMediator mediator,
            ILogger<EmailNotificationIntegrationEventHandler> logger)
        {
            _mediator = mediator;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(EmailNotificationIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                //TODO 

                await Task.CompletedTask;
            }
        }
    }
}
