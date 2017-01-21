using DriveHUD.Application.TableConfigurators.SeatArea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.TableConfigurators
{
    internal abstract class WinningPokerNetworkTableConfigurator : Poker888TableConfigurator
    {     
        protected override Dictionary<int, int[,]> GetPredefinedLabelPositions()
        {
            var predefinedLablelPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 355, 118 }, { 355, 409 } } },
                { 3, new int[,] { { 636, 211 }, { 355, 409 }, { 72, 211 } } },
                { 4, new int[,] { { 352, 105 }, { 638, 180 }, { 352, 411 }, { 96, 180 } } },
                { 6, new int[,] { { 352, 105 }, { 638, 180 }, { 638, 353 }, { 352, 411 }, { 96, 353 }, { 96, 180 } } },
                { 8, new int[,] { { 352, 105 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }  } },
                { 9, new int[,] { { 415, 118 }, { 636, 211 }, { 636, 318 }, { 490, 409 }, { 355, 409 }, { 220, 409 }, { 72, 318 }, { 72, 211 }, { 273, 118 }  } },
            };

            return predefinedLablelPositions;
        }  
    }
}
