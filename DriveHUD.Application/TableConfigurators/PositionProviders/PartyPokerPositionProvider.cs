using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    public class PartyPokerPositionProvider : IPositionProvider
    {
        public Dictionary<int, int[,]> Positions { get; }

        public PartyPokerPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {2, new int[,] {{352, 105}, {352, 411}}},
                {4, new int[,] {{352, 105}, {660, 256}, {352, 411}, {57, 256}}},
                {6, new int[,] {{352, 105}, {638, 180}, {638, 353}, {352, 411}, {96, 353}, {96, 180}}},
                {
                    8,
                    new int[,]
                        {{352, 105}, {529, 128}, {698, 258}, {529, 393}, {352, 411}, {196, 393}, {13, 258}, {194, 122}}
                },
                {
                    9,
                    new int[,]
                    {
                        {541, 113}, {670, 200}, {670, 342}, {541, 422}, {352, 411}, {161, 422}, {40, 342}, {40, 200},
                        {161, 113}
                    }
                },
                {
                    10,
                    new int[,]
                    {
                        {494, 106}, {639, 150}, {688, 269}, {639, 376}, {494, 422}, {235, 422}, {57, 376}, {15, 269},
                        {57, 150}, {235, 106}
                    }
                }
            };
        }

        public int GetOffsetX(int seats, int seat)
        {
            return -(25 / 2);
        }

        public int GetOffsetY(int seats, int seat)
        {
            return 38;
        }
    }
}
