using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.Entities;
using Rainbow.Architecture.Domain.SeedWork;
using System;
using System.Threading.Tasks;

namespace Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.IRepositories
{
    public interface IToDoItemRepository : IRepository<ToDoItem>
    {
        Task<ToDoItem> GetAsync(Guid todoId);

        Task<Guid> AddAsync(ToDoItem item);

        Task<bool> UpdateAsync(ToDoItem item);

    }
}
