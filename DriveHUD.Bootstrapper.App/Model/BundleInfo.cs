//-----------------------------------------------------------------------
// <copyright file="BundleInfo.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;

namespace DriveHUD.Bootstrapper.App.Model
{
    /// <summary>
    /// Contains information of the bundle.
    /// </summary>
    [XmlRoot("BootstrapperApplicationData", IsNullable = false, Namespace = "http://schemas.microsoft.com/wix/2010/BootstrapperApplicationData")]
    public class BundleInfo
    {      
        #region Properties

        [XmlElement("WixBundleProperties")]
        public BundleAttributes BundleAttributes { get; set; }

        [XmlElement("WixPackageProperties")]
        public Collection<PackageInfo> Packages { get; private set; }

        [XmlElement("WixBalCondition")]
        public Collection<BundleCondition> Conditions { get; private set; }

        #endregion

        #region Constructor

        public BundleInfo()
        {
            Packages = new Collection<PackageInfo>();
            Conditions = new Collection<BundleCondition>();
        }

        #endregion
    }
}