using System.Collections.Generic;

namespace ShippingQuotes.Business.Integrations.Vendor2
{
    public class RequestData
    {
        public string consignee { get; set; }
        public string consignor { get; set; }
        public IEnumerable<decimal> cartons { get; set;}
    }
}