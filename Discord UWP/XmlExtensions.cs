using System.Xml;
using System.Xml.Linq;

namespace Quarrel
{
    public static class XmlExtensions
    {
        public static Windows.Data.Xml.Dom.XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new Windows.Data.Xml.Dom.XmlDocument();
            var xReader = xDocument.CreateReader();
            string xmlString = xReader.ReadOuterXml();
            xmlDocument.LoadXml(xmlString);
            return xmlDocument;
        }

        /*public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }*/
    }
}
