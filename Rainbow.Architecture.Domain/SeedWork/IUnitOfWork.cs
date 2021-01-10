using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rainbow.Architecture.Domain.SeedWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}