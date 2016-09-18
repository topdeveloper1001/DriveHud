using DriveHUD.Application.ViewModels.Replayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;

namespace DriveHUD.Application.ControlTemplateSelectors
{
    public class ReplayerTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var radItem = (item as RadDiagramItem);

            if (radItem == null)
            {
                return null;
            }

            var classViewModel = radItem.Tag as ReplayerPlayerViewModel;

            if (classViewModel != null)
            {
                return PlayerPanelTemplate;
            }

            return null;
        }

        public DataTemplate PlayerPanelTemplate { get; set; }
    }
}
