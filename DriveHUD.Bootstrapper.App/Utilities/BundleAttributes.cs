using System.Xml.Serialization;
using System.Xml;

namespace DriveHUD.Bootstrapper.App.Utilities
{
    /// <summary>
    /// Contains attributes info of the bundle.
    /// </summary>
    public class BundleAttributes
    {
        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; }

    }
}
