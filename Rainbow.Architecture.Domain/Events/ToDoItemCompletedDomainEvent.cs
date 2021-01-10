using MediatR;
using System;

namespace Rainbow.Architecture.Domain.Events
{
    public class ToDoItemCompletedDomainEvent : INotification
    {
        public Guid ItemId { get; }

        public ToDoItemCompletedDomainEvent(Guid itemId)
        {
            ItemId = itemId;
        }
    }
}
