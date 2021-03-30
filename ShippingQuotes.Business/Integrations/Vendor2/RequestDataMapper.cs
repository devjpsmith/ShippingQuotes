using ShippingQuotes.Business.DTO;

namespace ShippingQuotes.Business.Integrations.Vendor2
{
    public static class RequestDataMapper
    {
        public static RequestData ToRequestData(this Shipment shipment)
            => new RequestData
            {
                consignee = shipment.Origin,
                consignor = shipment.Destination,
                cartons = shipment.Dimensions
            };
    }
}