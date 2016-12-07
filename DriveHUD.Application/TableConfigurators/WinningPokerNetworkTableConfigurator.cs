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
        protected override ITableSeatAreaConfigurator TableSeatAreaConfigurator
        {
            get
            {
                return new WinningPokerNetworkTableSeatAreaConfigurator();
            }
        }

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

        protected override Dictionary<int, int[,]> GetPredefinedMarkersPositions()
        {
            var predefinedPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 425, 87 }, { 425, 396 } } },
                { 3, new int[,] { { 614, 87 }, { 614, 396 }, { 147, 174 } } },
                { 4, new int[,] { { 425, 87 }, { 743, 174 }, { 425, 396 }, { 147, 174 } } },
                { 6, new int[,] { { 425, 87 }, { 743, 174 }, { 743, 316 }, { 425, 396 }, { 147, 316 }, { 147, 174 } } },
                { 8, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 743, 316 }, { 614, 396 }, { 235, 396 }, { 147, 316 }, { 147, 174 } } },
                { 9, new int[,] { { 425, 87 }, { 614, 87 }, { 743, 174 }, { 743, 316 }, { 614, 396 }, { 235, 396 }, { 147, 316 }, { 147, 174 }, { 235, 87 } } },
            };

            return predefinedPositions;
        }
    }
}
