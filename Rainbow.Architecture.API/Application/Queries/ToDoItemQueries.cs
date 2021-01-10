using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.Entities;
using Rainbow.Architecture.Infrastructure;
using System;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Application.Queries
{
    public class ToDoItemQueries : IToDoItemQueries
    {
        private readonly AppDbContext _dbContext;

        public ToDoItemQueries(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ToDoItem> GetOrderAsync(Guid id)
        {
            return await _dbContext.GetAsync<ToDoItem>(x => x.Id == id);
        }
    }
}
