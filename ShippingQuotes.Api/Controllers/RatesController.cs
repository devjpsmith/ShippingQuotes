using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ShippingQuotes.Business.DTO;
using ShippingQuotes.Business.Interfaces;

namespace ShippingQuotes.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class RatesController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public RatesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        [HttpPost]
        public async Task<decimal> Post(Shipment shipment)
        {
            return await _quoteService.GetBestQuoteAsync(shipment.Origin, shipment.Destination, shipment.Dimensions);
        }
    }
}