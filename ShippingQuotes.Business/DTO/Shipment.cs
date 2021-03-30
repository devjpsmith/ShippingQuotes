using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShippingQuotes.Business.DTO
{
    public class Shipment
    {
        [JsonProperty("source address")]        
        public string Origin {get;set;}

        [JsonProperty("destination address")]
        public string Destination { get;set;}

        [JsonProperty("carton dimensions")]
        public IEnumerable<decimal> Dimensions { get;set; }
    }
}