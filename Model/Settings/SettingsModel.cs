//-----------------------------------------------------------------------
// <copyright file="SettingsModel.cs" company="Ace Poker Solutions">
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
using System.Xml.Serialization;

namespace Model.Settings
{
    [Serializable]
    [XmlRoot("UserSettings")]
    public class SettingsModel : SettingsBase
    {
        [XmlElement("General")]
        public GeneralSettingsModel GeneralSettings { get; set; }

        [XmlElement("Currency")]
        public CurrencySettingsModel CurrencySettings { get; set; }

        [XmlElement("RakeBackAndBonuses")]
        public RakeBackSettingsModel RakeBackSettings { get; set; }

        [XmlElement("Database")]
        public DatabaseSettings DatabaseSettings { get; set; }

        [XmlElement("SiteSettings")]
        public SiteSettingsModel SiteSettings { get; set; }

        public SettingsModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            GeneralSettings = new GeneralSettingsModel();
            CurrencySettings = new CurrencySettingsModel();
            RakeBackSettings = new RakeBackSettingsModel();
            DatabaseSettings = new DatabaseSettings();
            SiteSettings = new SiteSettingsModel();
        }

        public override object Clone()
        {
            var model = (SettingsModel)this.MemberwiseClone();
            model.GeneralSettings = (GeneralSettingsModel)this.GeneralSettings.Clone();
            model.CurrencySettings = (CurrencySettingsModel)this.CurrencySettings.Clone();
            model.RakeBackSettings = (RakeBackSettingsModel)this.RakeBackSettings.Clone();
            model.DatabaseSettings = (DatabaseSettings)this.DatabaseSettings.Clone();
            model.SiteSettings = (SiteSettingsModel)this.SiteSettings.Clone();

            return model;
        }
    }
}