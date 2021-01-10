using Rainbow.Extensions.EventBus.Abstractions.Events;
using System;

namespace Rainbow.Architecture.API.Application.IntegrationEvents.Events
{
    public class EmailNotificationIntegrationEvent : IntegrationEvent
    {
        public Guid ItemId { get; }

        public EmailNotificationIntegrationEvent(Guid itemId) =>
            ItemId = itemId;
    }
}
