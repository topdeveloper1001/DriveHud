//-----------------------------------------------------------------------
// <copyright file="BundleAttributes.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Xml;
using System.Xml.Serialization;

namespace DriveHUD.Bootstrapper.App.Model
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