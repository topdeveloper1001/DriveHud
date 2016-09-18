using DriveHUD.Application.TableConfigurators;
using Model.Enums;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;

namespace DriveHUD.Application.ControlTemplateSelectors
{
    public class ClassTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var radItem = (item as RadDiagramItem);

            if (radItem == null)
            {
                return null;
            }

            if(radItem.DataContext is ITableSeatArea)
            {
                return null;
            }

            var classViewModel = radItem.DataContext as HudPlayerViewModel;

            if (classViewModel == null)
            {
                var hudType = radItem.Tag as HudType?;

                if(hudType == null)
                {
                    return null;
                }

                if (!hudType.HasValue)
                {
                    return PlayerPlaceTemplate;
                }

                if (hudType == HudType.Default)
                {
                    return RichHudTemplate;
                }

                return HudPlaceTemplate;
            }

            return PlayerPlaceTemplate;
        }

        public DataTemplate PlayerPlaceTemplate { get; set; }

        public DataTemplate HudPlaceTemplate { get; set; }

        public DataTemplate RichHudTemplate { get; set; }

    }
}