using DriveHUD.Entities;
using Model.Extensions;
using System.Windows;

namespace DriveHUD.Common.Wpf.DataTemplateSelectors
{
    public class FacingPreflopTemplateSelector : DynamicTemplateSelector
    {
        /// <summary>
        /// Overridden base method to allow the selection of the correct DataTemplate
        /// </summary>
        /// <param name="item">The item for which the template should be retrieved</param>
        /// <param name="container">The object containing the current item</param>
        /// <returns>The <see cref="DataTemplate"/> to use when rendering the <paramref name="item"/></returns>
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (!(container is UIElement))
                return base.SelectTemplate(item, container);

            var templates = this.Templates;
            if (templates == null || templates.Count == 0)
            {
                base.SelectTemplate(item, container);
            }

            var stat = item as ComparableCardsStatistic;
            if (stat != null)
            {

                foreach (var template in templates)
                {
                    if (((EnumFacingPreflop)template.Value).Equals(stat.Statistic.FacingPreflop))
                    {
                        return template.DataTemplate;
                    }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
