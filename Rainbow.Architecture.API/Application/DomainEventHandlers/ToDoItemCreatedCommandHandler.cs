using MediatR;
using Microsoft.Extensions.Logging;
using Rainbow.Architecture.API.Application.IntegrationEvents;
using Rainbow.Architecture.API.Application.IntegrationEvents.Events;
using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.IRepositories;
using Rainbow.Architecture.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Application.DomainEventHandlers
{
    // Regular CommandHandler
    public class ToDoItemCompletedDomainEventHandler : INotificationHandler<ToDoItemCompletedDomainEvent>
    {
        private readonly IToDoItemRepository _todoRepository;
        private readonly ILoggerFactory _logger;
        private readonly IAppIntegrationEventService _appIntegrationEventService;

        public ToDoItemCompletedDomainEventHandler(
            IToDoItemRepository todoRepository,
            ILoggerFactory logger,
            IAppIntegrationEventService appIntegrationEventService)
        {
            _todoRepository = todoRepository ?? throw new ArgumentNullException(nameof(todoRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appIntegrationEventService = appIntegrationEventService;
        }

        public async Task Handle(ToDoItemCompletedDomainEvent @event, CancellationToken cancellationToken)
        {
            _logger.CreateLogger<ToDoItemCreatedDomainEvent>().LogTrace("ToDoItemCompletedDomainEventHandler Start ......");
            var item = _todoRepository.GetAsync(@event.ItemId);

           var integrationEvent = new EmailNotificationIntegrationEvent(@event.ItemId);

           await _appIntegrationEventService.AddAndSaveEventAsync(integrationEvent);
        }
    }
}
