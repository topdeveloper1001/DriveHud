using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DriveHUD.Bootstrapper.App.Utilities
{
    /// <summary>
    /// Contains information about bundle conditions.
    /// </summary>
    public class BundleCondition
    {
        [XmlAttribute("Condition")]
        public string Condition { get; set; }

        [XmlAttribute("Message")]
        public string Message { get; set; }
    }
}
