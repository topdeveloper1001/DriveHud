using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;

namespace DriveHUD.Bootstrapper.App.Utilities
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
            string fileName = "BootstrapperApplicationData.xml";
            string fullFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);

            // Read the BootstrapperApplicationData.xml and create the BundleInfo object from that xml.
            var xmlSerializer = new XmlSerializer(typeof(BundleInfo));
            using (var fileStream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read))
            {
                var reader = XmlReader.Create(fileStream);
                return (BundleInfo)xmlSerializer.Deserialize(reader);
            }
        }
    }

}
