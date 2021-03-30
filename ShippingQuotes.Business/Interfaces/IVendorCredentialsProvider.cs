namespace ShippingQuotes.Business.Interfaces
{
    public interface IVendorCredentialsProvider
    {
         string GetUsernameByVendorName(string name);
         string GetPasswordByVendorName(string name);
         
    }
}