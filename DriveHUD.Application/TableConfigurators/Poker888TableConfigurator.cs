//-----------------------------------------------------------------------
// <copyright file="Poker888TableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels;
using DriveHUD.Entities;
using System.Collections.Generic;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.Linq;
using Model.Enums;
using System.Drawing;
using System.Windows.Media;
using System.Windows;

namespace DriveHUD.Application.TableConfigurators
{
    internal class Poker888TableConfigurator : CommonTableConfigurator
    {    
        protected override Dictionary<int, int[,]> GetPredefinedLabelPositions()
        {
            var predefinedLablelPositions = new Dictionary<int, int[,]>
            {                
                { 2, new int[,] { { 355, 118 }, { 355, 409 } } },                
                { 3, new int[,] { { 636, 262 }, { 355, 409 }, { 72, 262 } } },                
                { 4, new int[,] { { 355, 118 }, { 636, 262 }, { 355, 409 }, { 72, 262 } } },                
                { 5, new int[,] { { 490, 118 }, { 636, 318 }, { 355, 409 }, { 72, 318 }, { 220, 118 } } },                
                { 6, new int[,] { { 422, 118 }, { 636, 262 }, { 422, 409 }, { 264, 409 }, { 72, 262 }, { 264, 118 } } },                
                { 8, new int[,] { { 422, 118 }, { 636, 211 }, { 636, 318 }, { 422, 409 }, { 264, 409 }, { 72, 318 }, { 72, 211 }, { 264, 118 } } },                
                { 9, new int[,] { { 415, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 273, 118 }  } },                
                { 10, new int[,] { { 490, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 220, 118 }, { 355, 118 } } }
            };

            return predefinedLablelPositions;
        }
    }
}