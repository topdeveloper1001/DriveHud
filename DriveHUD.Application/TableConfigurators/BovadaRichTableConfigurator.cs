//-----------------------------------------------------------------------
// <copyright file="BovadaRichTableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using Model.Enums;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    internal class BovadaRichTableConfigurator : BovadaTableConfigurator
    {      
        public override int HudElementWidth
        {
            get
            {
                return (int)HudDefaultSettings.BovadaRichHudElementWidth;
            }
        }

        public override int HudElementHeight
        {
            get
            {
                return (int)HudDefaultSettings.BovadaRichHudElementHeight;
            }
        }

        protected override RadDiagramShape CreateRadDiagramShape(HudElementViewModel viewModel)
        {
            var label = new RadDiagramShape()
            {
                DataContext = viewModel,
                StrokeThickness = 0,
                BorderThickness = new Thickness(0),
                IsEnabled = true,
                Background = null,
                IsRotationEnabled = false,
                Tag = HudType.Default,
                Padding = new Thickness(0),
                IsDraggingEnabled = false
            };

            return label;
        }

        protected override Dictionary<int, int[,]> GetPredefinedPositions()
        {
            var predefinedPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 334, 45 }, { 334, 389 } } },
                { 6, new int[,] { { 334, 45 }, { 631, 131 }, { 631, 319 }, { 334, 399 }, { 36, 319 }, { 36, 131 } } },
                { 9, new int[,] { { 451, 45 }, { 632, 116 }, { 645, 263 }, { 524, 384 }, { 333, 398 }, { 148, 384 }, { 22, 263 }, { 38, 116 }, { 218, 45 } } }
            };

            return predefinedPositions;
        }
    }
}