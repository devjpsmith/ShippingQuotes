using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShippingQuotes.Business.DTO;
using ShippingQuotes.Business.Interfaces;

namespace ShippingQuotes.Business.Integrations.Vendor2
{
    public class Vendor2Service : IVendorService
    {
        private const string _contentType = "application/json";

        private readonly IApiService _apiService;
        private readonly IVendorUrlProvider _urlProvider;
        private readonly IVendorCredentialsProvider _credentialsProvider;
        private ILogger<Vendor2Service> _logger;

        public Vendor2Service(IApiService apiService, IVendorUrlProvider urlProvider, IVendorCredentialsProvider credentialsProvider, ILogger<Vendor2Service> logger)
        {
            _apiService = apiService;
            _urlProvider = urlProvider;
            _credentialsProvider = credentialsProvider;
            _logger = logger;
        }

        public async Task<decimal?> GetQuoteAsync(Shipment shipment)
        {
            var requestData = shipment.ToRequestData();
            var body = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(body);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse(_contentType);
            var url = _urlProvider.GetVendorUrlByName("Vendor2");
            var response = await _apiService.PostAsync(url, content, GetAuthorizationHeaderValue());
            return ParseResponse(response);
        }

        protected AuthenticationHeaderValue GetAuthorizationHeaderValue()
        {
            string username = _credentialsProvider.GetUsernameByVendorName("Vendor2"),
                password = _credentialsProvider.GetPasswordByVendorName("Vendor2");
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new AuthenticationHeaderValue("Basic", null);
            }
            var bytes = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password));
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
        }

        protected decimal? ParseResponse(string response)
        {
            try
            {
                return JsonConvert.DeserializeAnonymousType(response, new { amount = (decimal?)null })?.amount;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Response: {response}", response);
            }
            return null;
        }
    }
}