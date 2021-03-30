using ShippingQuotes.Business.Interfaces;

namespace ShippingQuotes.Business.Tests.Mocks
{
    public class CredentialsProviderMock : IVendorCredentialsProvider
    {
        private readonly string _username;
        private readonly string _password;

        public CredentialsProviderMock(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string GetPasswordByVendorName(string name) => _password;

        public string GetUsernameByVendorName(string name) => _username;
    }
}