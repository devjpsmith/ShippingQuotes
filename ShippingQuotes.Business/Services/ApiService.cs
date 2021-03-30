using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ShippingQuotes.Business.Interfaces;

namespace ShippingQuotes.Business.Services
{
    public class ApiService : IApiService
    {
        private readonly ILogger<ApiService> _logger;

        public ApiService(ILogger<ApiService> logger)
        {
            _logger = logger;
        }

        public async Task<string> PostAsync(string url, HttpContent content, AuthenticationHeaderValue auth)
        {
            // initialize the return value as empty, in case something goes wrong
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                if (string.IsNullOrEmpty(auth.Parameter) == false)
                {
                    // only add authorization header if authorization token is not null
                    client.DefaultRequestHeaders.Authorization = auth;
                }
                try
                {
                    using (var result = await client.PostAsync(url, content))
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            response = await result.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            _logger.LogWarning("Request to {url} resulted in StatusCode: {StatusCode}", url, result.StatusCode);
                        }
                    }
                }
                catch
                {

                }
            }
            return response;
        }
    }
}