using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.Entities;
using System;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Application.Queries
{
    public interface IToDoItemQueries
    {
        Task<ToDoItem> GetOrderAsync(Guid id);
    }
}
