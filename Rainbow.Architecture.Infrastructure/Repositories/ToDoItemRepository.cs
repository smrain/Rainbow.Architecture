using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.Entities;
using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.IRepositories;
using Rainbow.Architecture.Domain.SeedWork;
using System;
using System.Threading.Tasks;

namespace Rainbow.Architecture.Infrastructure.Repositories
{
    public class ToDoItemRepository : IToDoItemRepository
    {
        private readonly AppDbContext _context;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public ToDoItemRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ToDoItem> GetAsync(Guid id)
        {
            return await _context.GetAsync<ToDoItem>(o => o.Id == id);
        }

        public async Task<Guid> AddAsync(ToDoItem item)
        {
            return await _context.AddAsync(item);
        }

        public async Task<bool> UpdateAsync(ToDoItem item)
        {
            return await _context.UpdateAsync(item);
        }
    }
}