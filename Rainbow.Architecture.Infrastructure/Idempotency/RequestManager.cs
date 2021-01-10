using Rainbow.Architecture.Domain.Exceptions;
using System;
using System.Threading.Tasks;

namespace Rainbow.Architecture.Infrastructure.Idempotency
{
    public class RequestManager : IRequestManager
    {
        private readonly AppDbContext _context;

        public RequestManager(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<bool> ExistAsync(Guid id)
        {
            var request = await _context.GetAsync<ClientRequest>(x => x.Id == id.ToString(), c => new { c.Id });

            return request != null;
        }

        public async Task CreateRequestForCommandAsync<T>(Guid id)
        {
            var exists = await ExistAsync(id);

            var request = exists ?
                throw new AppDomainException($"Request with {id} already exists") :
                new ClientRequest()
                {
                    Id = id.ToString(),
                    Name = typeof(T).Name,
                    Time = DateTime.UtcNow
                };

            await _context.AddAsync(request);

            await _context.SaveChangeAsync();
        }
    }
}
