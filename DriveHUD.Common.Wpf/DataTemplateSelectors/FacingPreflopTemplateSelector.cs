//-----------------------------------------------------------------------
// <copyright file="FacingPreflopTemplateSelector.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Data;
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
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (!(container is UIElement))
            {
                return base.SelectTemplate(item, container);
            }

            var templates = Templates;

            if (templates == null || templates.Count == 0)
            {
                base.SelectTemplate(item, container);
            }

            var reportHand = item as ReportHandViewModel;

            if (reportHand != null)
            {
                foreach (var template in templates)
                {
                    if (((EnumFacingPreflop)template.Value).Equals(reportHand.FacingPreflop))
                    {
                        return template.DataTemplate;
                    }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}