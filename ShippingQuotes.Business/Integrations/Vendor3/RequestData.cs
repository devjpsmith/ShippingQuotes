using System.Collections.Generic;
using System.Xml.Serialization;

namespace ShippingQuotes.Business.Integrations.Vendor3
{
    // [XmlNamespaceDeclarations = ]
    [XmlRoot("xml")]
    public class RequestData
    {
        public string source { get; set; }
        public string destination { get; set; }
        [XmlArray]
        [XmlArrayItem("package")]
        public List<decimal> packages { get; set; }
    }
}