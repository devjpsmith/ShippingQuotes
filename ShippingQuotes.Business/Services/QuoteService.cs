using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingQuotes.Business.DTO;
using ShippingQuotes.Business.Interfaces;

namespace ShippingQuotes.Business.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IEnumerable<IVendorService> _vendorServices;

        public QuoteService(IEnumerable<IVendorService> vendorServices)
        {
            _vendorServices = vendorServices;
        }

        public async Task<decimal> GetBestQuoteAsync(string origin, string destination, IEnumerable<decimal> dimensions)
        {
            List<decimal?> quotes = new List<decimal?>();
            var shipment = new Shipment { Origin = origin, Destination = destination, Dimensions = dimensions };
            
            var tasks =_vendorServices.Select(async (v) => quotes.Add(await v.GetQuoteAsync(shipment)));
            await Task.WhenAll(tasks.ToArray());
            return quotes.Where(q => q != null)
                         .Select(q => (decimal)q)
                         .Min();
        }
    }
}