using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Xml;

namespace DriveHUD.Bootstrapper.App.Utilities
{
    /// <summary>
    /// Contains information of the bundle.
    /// </summary>
    [XmlRoot("BootstrapperApplicationData", IsNullable = false, Namespace = "http://schemas.microsoft.com/wix/2010/BootstrapperApplicationData")]
    public class BundleInfo
    {
        #region Private Members

        private Collection<PackageInfo> _packages;

        private Collection<BundleCondition> _conditions;

        #endregion

        #region Properties

        [XmlElement("WixBundleProperties")]
        public BundleAttributes BundleAttributes { get; set; }

        [XmlElement("WixPackageProperties")]
        public Collection<PackageInfo> Packages
        {
            get
            {
                return _packages;
            }
        }

        [XmlElement("WixBalCondition")]
        public Collection<BundleCondition> Conditions
        {
            get
            {
                return _conditions;
            }
        }

        #endregion

        #region Constructor

        public BundleInfo()
        {
            _packages = new Collection<PackageInfo>();
            _conditions = new Collection<BundleCondition>();
        }

        #endregion
    }
}
