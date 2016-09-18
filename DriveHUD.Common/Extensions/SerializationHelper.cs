//-----------------------------------------------------------------------
// <copyright file="SerializationHelper.cs" company="Ace Poker Solutions">
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
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DriveHUD.Common.Extensions
{
    /// <summary>
    /// Helper for data serialization
    /// </summary>
    public class SerializationHelper
    {
        /// <summary>
        /// Serialize an object into an XML string
        /// </summary>
        /// <typeparam name="T">Type of object to serialize.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <param name="enc">Encoding of the serialized output.</param>
        /// <returns>Serialized (xml) object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static string SerializeObject<T>(T obj, Encoding enc)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            using (var ms = new MemoryStream())
            {
                var xmlWriterSettings = new XmlWriterSettings()
                {
                    // If set to true XmlWriter would close MemoryStream automatically and using would then do double dispose
                    // Code analysis does not understand that. That's why there is a suppress message.
                    CloseOutput = false,
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = false,
                    Indent = true
                };

                using (var xw = XmlWriter.Create(ms, xmlWriterSettings))
                {
                    var s = new XmlSerializer(typeof(T));
                    s.Serialize(xw, obj, new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) }));
                }

                var result = enc.GetString(ms.ToArray());

                var _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

                if (result.StartsWith(_byteOrderMarkUtf8))
                {
                    result = result.Remove(0, _byteOrderMarkUtf8.Length);
                }

                return result;
            }
        }
    }
}