using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ShippingQuotes.Business.DTO;
using ShippingQuotes.Business.Interfaces;
using ShippingQuotes.Business.Helpers;
using System.Xml;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ShippingQuotes.Business.Integrations.Vendor3
{
    public class Vendor3Service : IVendorService
    {
        
        private readonly IApiService _apiService;
        private readonly IVendorUrlProvider _urlProvider;
        private readonly IVendorCredentialsProvider _credentialsProvider;
        private ILogger<Vendor3Service> _logger;

        public Vendor3Service(IApiService apiService, IVendorUrlProvider urlProvider, IVendorCredentialsProvider credentialsProvider, ILogger<Vendor3Service> logger)
        {
            _apiService = apiService;
            _urlProvider = urlProvider;
            _credentialsProvider = credentialsProvider;
            _logger = logger;
        }

        public async Task<decimal?> GetQuoteAsync(Shipment shipment)
        {
            var requestData = shipment.ToRequestData();
            var url = _urlProvider.GetVendorUrlByName("Vendor3");
            var body = XmlSerializationHelper.GetXml(requestData);
            var content = new StringContent(body);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xml");
            var response = await _apiService.PostAsync(url, content, GetAuthorizationHeaderValue());
            return ParseResponse(response);
        }

        protected AuthenticationHeaderValue GetAuthorizationHeaderValue()
        {
            string username = _credentialsProvider.GetUsernameByVendorName("Vendor3"),
                password = _credentialsProvider.GetPasswordByVendorName("Vendor3");
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return new AuthenticationHeaderValue("Basic", null);
            }
            var bytes = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password));
            return new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
        }

        protected decimal? ParseResponse(string response)
        {
            using (var sr = new StringReader(response))
            {
                try
                {
                    XmlReader xr = XmlReader.Create(sr);
                var read = false;
                while (xr.Read())
                {
                    if (xr.NodeType == XmlNodeType.Element && xr.Name.Equals("quote"))
                    {
                        read = true;
                    }
                    if (xr.NodeType == XmlNodeType.Text && read)
                    {
                        decimal val;
                        if (decimal.TryParse(xr.Value, out val))
                        {
                            return val;
                        }
                    }
                    if (xr.NodeType == XmlNodeType.EndElement)
                    {
                        read = false;
                    }
                }
                }
                catch (XmlException ex)
                {
                    _logger.LogError(ex, "Response: {response}", response);
                }
            }
            return null;
        }
    }
}