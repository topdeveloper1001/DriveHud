﻿using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DriveHUD.Common.Wpf.DataTemplateSelectors
{
    public class OmahaHandGridTemplateSelector : DynamicTemplateSelector
    {
        /// <summary>
        /// Overriden base method to allow the selection of the correct DataTemplate
        /// </summary>
        /// <param name="item">The item for which the template should be retrieved</param>
        /// <param name="container">The object containing the current item</param>
        /// <returns>The <see cref="DataTemplate"/> to use when rendering the <paramref name="item"/></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(container is UIElement))
                return base.SelectTemplate(item, container);

            var templates = this.Templates;
            if (templates == null || templates.Count == 0)
            {
                base.SelectTemplate(item, container);
            }

            var handGridItem = item as OmahaHandGridItem;
            if (handGridItem != null)
            {

                foreach (var template in templates)
                {
                    if (((Model.Enums.OmahaHoleCards)template.Value).Equals(handGridItem.HoleCards))
                    {
                        return template.DataTemplate;
                    }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
