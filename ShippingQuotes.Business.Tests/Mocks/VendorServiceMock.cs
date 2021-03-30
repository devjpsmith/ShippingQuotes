using System.Threading.Tasks;
using ShippingQuotes.Business.DTO;
using ShippingQuotes.Business.Interfaces;

namespace ShippingQuotes.Business.Tests.Mocks
{
    public class VendorServiceMock : IVendorService
    {
        public decimal? TestQuote { get; set; }
        public Task<decimal?> GetQuoteAsync(Shipment shipment)
        {
            return Task.FromResult<decimal?>(TestQuote);
        }
    }
}