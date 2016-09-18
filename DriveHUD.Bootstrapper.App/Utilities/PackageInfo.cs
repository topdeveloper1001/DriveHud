using System.Xml.Serialization;
using System.Xml;

namespace DriveHUD.Bootstrapper.App.Utilities
{
    /// <summary>
    /// Contains information about the packages whitch will be installed.
    /// </summary>
    public class PackageInfo
    {
        [XmlAttribute("Package")]
        public string Id { get; set; }

        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }
    }
}
