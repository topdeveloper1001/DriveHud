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

using DriveHUD.Entities;
using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators
{
    internal class Poker888TableConfigurator : CommonTableConfigurator
    {
        public override EnumPokerSites Type
        {
            get { return EnumPokerSites.Poker888; }
        }

        protected override Dictionary<int, int[,]> GetPredefinedLabelPositions()
        {
            var predefinedLablelPositions = new Dictionary<int, int[,]>
            {
                // done
                { 2, new int[,] { { 355, 118 }, { 355, 409 } } },
                // done
                { 3, new int[,] { { 636, 262 }, { 355, 409 }, { 72, 262 } } },
                // done
                { 4, new int[,] { { 355, 118 }, { 636, 262 }, { 355, 409 }, { 72, 262 } } },
                // done
                { 5, new int[,] { { 415, 118 }, { 636, 318 }, { 355, 409 }, { 72, 318 }, { 220, 118 } } },
                // done
                { 6, new int[,] { { 422, 118 }, { 636, 262 }, { 422, 409 }, { 264, 409 }, { 72, 262 }, { 264, 118 } } },
                // done
                { 8, new int[,] { { 422, 118 }, { 636, 211 }, { 636, 318 }, { 422, 409 }, { 264, 409 }, { 72, 318 }, { 72, 211 }, { 264, 118 } } },
                // done
                { 9, new int[,] { { 415, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 273, 118 }  } },
                // done
                { 10, new int[,] { { 490, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 220, 118 }, { 355, 118 } } }
            };

            return predefinedLablelPositions;
        }

        protected override Dictionary<int, int[,]> GetPredefinedMarkersPositions()
        {
            var predefinedPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 425, 87 }, { 425, 396 } } },
                { 3, new int[,] { { 425, 87 }, { 615, 396 }, { 235, 396 } } },
                { 4, new int[,] { { 743, 174 }, { 743, 316 }, { 147, 316 }, { 147, 174 } } },
                { 5, new int[,] { { 743, 174 }, { 743, 316 }, { 147, 316 }, { 147, 174 }, { 147, 174 } } },
                { 6, new int[,] { { 425, 87 }, { 743, 174 }, { 743, 316 }, { 425, 396 }, { 147, 316 }, { 147, 174 } } },
                { 8, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 614, 396 }, { 425, 396 }, { 235, 396 }, { 147, 174 }, { 235, 87 }  } },
                { 9, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 743, 316 }, { 614, 396 }, { 235, 396 }, { 147, 316 }, { 147, 174 }, { 235, 87 } } },
                { 10, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 743, 316 }, { 614, 396 }, { 425, 396 }, { 235, 396 }, { 147, 316 }, { 147, 174 }, { 235, 87 } } }
            };

            return predefinedPositions;
        }
    }
}