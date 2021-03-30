using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShippingQuotes.Business.Integrations.Vendor1
{
    public class RequestData
    {
        [JsonProperty(PropertyName="contact address")]
        public string contactAddress { get; set; }
        [JsonProperty(PropertyName="warehouse address")]
        public string warehouseAddress { get; set; }
        [JsonProperty(PropertyName="package dimensions")]
        public IEnumerable<decimal> packageDimensions { get; set; }
    }
}