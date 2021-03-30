using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ShippingQuotes.Business.Interfaces
{
    public interface IApiService
    {
         Task<string> PostAsync(string url, HttpContent content, AuthenticationHeaderValue auth);
    }
}