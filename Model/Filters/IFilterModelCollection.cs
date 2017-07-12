//-----------------------------------------------------------------------
// <copyright file="IFilterModelCollection.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Filters
{
    public class IFilterModelCollection : ObservableCollection<IFilterModel>, IXmlSerializable
    {
        private const string AssemblyQualifiedName = "AssemblyQualifiedName";

        public IFilterModelCollection() : base() { }

        public IFilterModelCollection(IEnumerable<IFilterModel> collection) : base(collection) { }

        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement("FilterModelCollection");

                while (reader.IsStartElement(nameof(IFilterModel)))
                {
                    var assemblyName = reader.GetAttribute(AssemblyQualifiedName);
                    var type = Type.GetType(assemblyName);
                    var serializer = new XmlSerializer(type);

                    reader.ReadStartElement(nameof(IFilterModel));
                    Add((IFilterModel)serializer.Deserialize(reader));
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
                writer.WriteStartElement(nameof(IFilterModel));
                writer.WriteAttributeString(AssemblyQualifiedName, filterModel.GetType().AssemblyQualifiedName);

                var xmlSerializer = new XmlSerializer(filterModel.GetType());
                xmlSerializer.Serialize(writer, filterModel);

                writer.WriteEndElement();
            }
        }
    }
}