using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Infrastructure.Services
{
    public class WebRequestClient: IWebRequestClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AppSettings _settings;
        public WebRequestClient(IHttpClientFactory httpClientFactory, IOptions<AppSettings> settings)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings.Value;
        }


        public async Task<TResult> GetAsync<TResult>(string url)
        {
            var client = _httpClientFactory.CreateClient("GrantClient");
            var response = await client.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TResult>(json);
            return result;
        }
    }
}
