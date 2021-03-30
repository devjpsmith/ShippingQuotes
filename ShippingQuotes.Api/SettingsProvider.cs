using Microsoft.Extensions.Configuration;
using ShippingQuotes.Business.Interfaces;

namespace ShippingQuotes.Api
{
    public class SettingsProvider : IVendorUrlProvider, IVendorCredentialsProvider
    {
        private readonly IConfiguration _config;

        public SettingsProvider(IConfiguration config)
        {
            _config = config;
        }

        public string GetPasswordByVendorName(string name)
        {
            return _config[string.Format("{0}_USERNAME", name.ToUpperInvariant())];
        }

        public string GetUsernameByVendorName(string name)
        {
            return _config[string.Format("{0}_PASSWORD", name.ToUpperInvariant())];
        }

        public string GetVendorUrlByName(string name)
        {
            return _config[string.Format("ApiUrls:{0}", name)];
        }
    }
}