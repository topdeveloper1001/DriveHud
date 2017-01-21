using System;
using System.Collections.Generic;

namespace DriveHUD.Application.ViewModels.Layouts
{
    [Serializable]
    public class HudLayoutMappings
    {
        public List<HudLayoutMapping> Mappings { get; set; }

        public HudLayoutMappings()
        {
            Mappings = new List<HudLayoutMapping>();
        }
    }
}