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
using System.Xml;

namespace DriveHUD.Bootstrapper.App.Model
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