using MediatR;
using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.Entities;
using System;

namespace Rainbow.Architecture.Domain.Events
{
    public class ToDoItemCreatedDomainEvent : INotification
    {
        public ToDoItem CreatedItem { get; }

        public ToDoItemCreatedDomainEvent(ToDoItem createdItem)
        {
            CreatedItem = createdItem;
        }
    }
}
