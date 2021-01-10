using Rainbow.Architecture.Domain.Events;
using Rainbow.Architecture.Domain.SeedWork;
using System;

namespace Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.Entities
{
    public class ToDoItem : Entity<Guid>, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool IsDone { get; private set; }

        public ToDoItem(string title, string description)
        {
            base.Id = (base.Id == Guid.Empty) ? base.Id : Guid.NewGuid();
            Title = title;
            Description = description;

            var @event = new ToDoItemCreatedDomainEvent(this);
            this.AddDomainEvent(@event);
        }

        public void MarkCompleted()
        {
            IsDone = true;
            var @event = new ToDoItemCompletedDomainEvent(this.Id);
            this.AddDomainEvent(@event);
        }
    }

}
