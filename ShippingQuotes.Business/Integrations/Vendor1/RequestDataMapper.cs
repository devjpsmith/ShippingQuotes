using ShippingQuotes.Business.DTO;

namespace ShippingQuotes.Business.Integrations.Vendor1
{
    public static class RequestDataMapper
    {
        public static RequestData ToRequestData(this Shipment shipment)
        {
            return new RequestData
            {
                contactAddress = shipment.Origin,
                warehouseAddress = shipment.Destination,
                packageDimensions = shipment.Dimensions
            };
        }
    }
}