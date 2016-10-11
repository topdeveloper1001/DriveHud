using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Filters
{
    public class IFilterModelCollection : ObservableCollection<IFilterModel>, IXmlSerializable
    {
        public IFilterModelCollection() : base() { }

        public IFilterModelCollection(IEnumerable<IFilterModel> collection) : base(collection) { }

        public System.Xml.Schema.XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement("FilterModelCollection");
                while (reader.IsStartElement("IFilterModel"))
                {
                    Type type = Type.GetType(reader.GetAttribute("AssemblyQualifiedName"));
                    XmlSerializer serial = new XmlSerializer(type);

                    reader.ReadStartElement("IFilterModel");
                    this.Add((IFilterModel)serial.Deserialize(reader));
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
            else
            {
                reader.Skip();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (IFilterModel filterModel in this)
            {
                writer.WriteStartElement("IFilterModel");
                writer.WriteAttributeString
                ("AssemblyQualifiedName", filterModel.GetType().AssemblyQualifiedName);
                XmlSerializer xmlSerializer = new XmlSerializer(filterModel.GetType());
                xmlSerializer.Serialize(writer, filterModel);
                writer.WriteEndElement();
            }
        }
    }
}
