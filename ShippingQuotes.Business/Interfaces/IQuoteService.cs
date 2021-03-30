using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShippingQuotes.Business.Interfaces
{
    public interface IQuoteService
    {
         Task<decimal> GetBestQuoteAsync(string source, string destination, IEnumerable<decimal> dimensions);
    }
}