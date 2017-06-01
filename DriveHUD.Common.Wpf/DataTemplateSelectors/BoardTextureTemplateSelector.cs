//-----------------------------------------------------------------------
// <copyright file="BoardTextureTemplateSelector.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Filters;
using System.Windows;

namespace DriveHUD.Common.Wpf.DataTemplateSelectors
{
    public class BoardTextureTemplateSelector : DynamicTemplateSelector
    {
        /// <summary>
        /// Overridden base method to allow the selection of the correct DataTemplate
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

            var boardTextureItem = item as BoardTextureItem;
            if (boardTextureItem != null)
            {

                foreach (var template in templates)
                {
                    if (((Model.Enums.BoardTextures)template.Value).Equals(boardTextureItem.BoardTexture))
                    {
                        return template.DataTemplate;
                    }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}