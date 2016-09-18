using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [XmlElement("ReferredSeat")]
        public PreferredSeatSettingsModel PreferredSeatSettings { get; set; }

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
            PreferredSeatSettings = new PreferredSeatSettingsModel();
            SiteSettings = new SiteSettingsModel();
        }

        public override object Clone()
        {
            var model = (SettingsModel)this.MemberwiseClone();
            model.GeneralSettings = (GeneralSettingsModel)this.GeneralSettings.Clone();
            model.CurrencySettings = (CurrencySettingsModel)this.CurrencySettings.Clone();
            model.RakeBackSettings = (RakeBackSettingsModel)this.RakeBackSettings.Clone();
            model.DatabaseSettings = (DatabaseSettings)this.DatabaseSettings.Clone();
            model.PreferredSeatSettings = (PreferredSeatSettingsModel)this.PreferredSeatSettings.Clone();
            model.SiteSettings = (SiteSettingsModel)this.SiteSettings.Clone();

            return model;
        }
    }
}
