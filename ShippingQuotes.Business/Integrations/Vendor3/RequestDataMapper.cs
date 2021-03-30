using System.Linq;
using ShippingQuotes.Business.DTO;

namespace ShippingQuotes.Business.Integrations.Vendor3
{
    public static class RequestDataMapper
    {
        public static RequestData ToRequestData(this Shipment shipment)
            => new RequestData
            {
                source = shipment.Origin,
                destination = shipment.Destination,
                packages = shipment.Dimensions?.ToList()
            };
    }
}