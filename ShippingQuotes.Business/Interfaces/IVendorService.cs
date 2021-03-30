using System.Threading.Tasks;
using ShippingQuotes.Business.DTO;

namespace ShippingQuotes.Business.Interfaces
{
    public interface IVendorService
    {
        
        Task<decimal?> GetQuoteAsync(Shipment shipment);
    }
}