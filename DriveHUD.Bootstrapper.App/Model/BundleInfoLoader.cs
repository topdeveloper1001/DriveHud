//-----------------------------------------------------------------------
// <copyright file="PackageInfo.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;

namespace DriveHUD.Bootstrapper.App.Model
{
    /// <summary>
    /// Loads the bundle information from the xml file: "BootstrapperApplicationData.xml"
    /// </summary>
    public class BundleInfoLoader
    {
        /// <summary>
        /// Loads the bundle information.
        /// </summary>
        /// <returns></returns>
        public static BundleInfo Load()
        {
            const string fileName = "BootstrapperApplicationData.xml";

            try
            {
                var fullFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);

                // Read the BootstrapperApplicationData.xml and create the BundleInfo object from that xml.
                var xmlSerializer = new XmlSerializer(typeof(BundleInfo));

                using (var fileStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
                {
                    var reader = XmlReader.Create(fileStream);
                    return (BundleInfo)xmlSerializer.Deserialize(reader);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}