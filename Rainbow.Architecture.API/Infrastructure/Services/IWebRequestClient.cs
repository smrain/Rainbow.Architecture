using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Infrastructure.Services
{
    public interface IWebRequestClient
    {
        Task<TResult> GetAsync<TResult>(string url);
    }
}
