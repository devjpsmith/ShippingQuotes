using System.IO;
using System.Xml.Serialization;

namespace ShippingQuotes.Business.Helpers
{
    public static class XmlSerializationHelper
    {
        public static string GetXml(object obj)
        {
            string xml;
            var x = new XmlSerializer(obj.GetType());
            
            using (var ms = new MemoryStream())
            using (var sr = new StreamReader(ms))
            {
                x.Serialize(ms, obj);
                ms.Position = 0;
                xml = sr.ReadToEnd();
            }
            return xml;
        }
    }
}