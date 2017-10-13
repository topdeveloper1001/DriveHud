#region Usings

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

#endregion

namespace DriveHUD.PlayerXRay.BusinessHelper
{
    /// <summary>
    /// Provides serialization and file IO support for all domain
    /// classes in this namespace.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Serializes the <c>Obj</c> to an XML string.
        /// </summary>
        /// <param name="obj">
        /// The object to serialize.</param>
        /// <param name="objType">The object type.</param>
        /// <returns>
        /// The serialized object XML string.
        /// </returns>
        public static string ToXml(object obj, Type objType)
        {
            var ser = new XmlSerializer(objType);

            var memStream = new MemoryStream();

            var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 1,
                IndentChar = Convert.ToChar(9),
                Namespaces = true
            };

            ser.Serialize(xmlWriter, obj);

            xmlWriter.Close();
            memStream.Close();

            var xml = Encoding.UTF8.GetString(memStream.GetBuffer());

            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            xml = xml.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n", string.Empty);
            xml = xml.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ", string.Empty);

            return xml;
        }

        /// <summary>
        /// Creates an object from an XML string.
        /// </summary>
        /// <param name="xml">
        /// XML string to serialize.</param>
        /// <param name="objType">The object type to create.</param>
        /// <returns>
        /// An object of type <c>ObjType</c>.
        /// </returns>
        public static object FromXml(string xml, Type objType)
        {
            XmlSerializer ser = new XmlSerializer(objType);
            StringReader stringReader = new StringReader(xml);
            XmlTextReader xmlReader = new XmlTextReader(stringReader);
            object obj = ser.Deserialize(xmlReader);
            xmlReader.Close();
            stringReader.Close();

            return obj;
        }
    }
}