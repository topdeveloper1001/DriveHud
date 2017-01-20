using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    public class CommonPositionProvider : IPositionProvider
    {
        public Dictionary<int, int[,]> Positions { get; }

        public CommonPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {2, new[,] {{347, 133}, {347, 418}}},
                {3, new[,] {{352, 105}, {541, 422}, {161, 422}}},
                {4, new[,] {{340, 143}, {657, 291}, {360, 442}, {41, 286}}},
                {5, new[,] {{352, 105}, {660, 256}, {352, 411}, {57, 256}, {0, 0}}},
                {6, new[,] {{342, 123}, {630, 218}, {626, 391}, {343, 413}, {78, 377}, {79, 218}}},
                {
                    8,
                    new[,]
                        {{340, 143}, {517, 166}, {686, 296}, {517, 431}, {340, 449}, {184, 431}, {1, 296}, {182, 160}}
                },
                {
                    9,
                    new[,]
                    {
                        {459, 122}, {646, 189}, {659, 306}, {532, 419}, {349, 444}, {159, 420}, {26, 307}, {38, 193}, {225, 121}
                    }
                },
                {
                    10,
                    new[,]
                    {
                        {340, 143}, {517, 166}, {667, 238}, {667, 347}, {517, 431}, {340, 449}, {184, 431}, {15, 347},
                        {21, 238}, {182, 160}
                    }
                }
            };
        }

        public int GetOffsetX(int seats, int seat)
        {
            return 0;
        }

        public int GetOffsetY(int seats, int seat)
        {
            return 0;
        }
    }

    public class CommonRichPositionProvider : IPositionProvider
    {
        public Dictionary<int, int[,]> Positions { get; }

        public CommonRichPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {2, new[,] {{352, 105}, {352, 411}}},
                {3, new[,] {{352, 105}, {541, 422}, {161, 422}}},
                {4, new[,] {{352, 105}, {660, 256}, {352, 411}, {57, 256}}},
                {5, new[,] {{352, 105}, {660, 256}, {352, 411}, {57, 256}, {0, 0}}},
                {6, new[,] {{352, 105}, {638, 180}, {638, 353}, {352, 411}, {96, 353}, {96, 180}}},
                {
                    8,
                    new[,]
                        {{352, 105}, {529, 128}, {698, 258}, {529, 393}, {352, 411}, {196, 393}, {13, 258}, {194, 122}}
                },
                {
                    9,
                    new[,]
                    {
                        {415, 118}, {636, 211}, {636, 318}, {490, 409}, {355, 409}, {220, 409}, {72, 318}, {72, 211},
                        {273, 118}
                    }
                },
                {
                    10,
                    new[,]
                    {
                        {352, 105}, {529, 128}, {678, 200}, {678, 309}, {529, 393}, {352, 411}, {196, 393}, {27, 309},
                        {33, 200}, {194, 122}
                    }
                }
            };
        }

        private bool IsRightOriented(int seats, int seat)
        {
            return (seats > 6 && seat < 5) || (seats < 7 && seats > 2 && seat < 3) || (seats < 3 && seat < 1);
        }

        public int GetOffsetX(int seats, int seat)
        {
            return IsRightOriented(seats, seat) ? 4 : 36;
        }

        public int GetOffsetY(int seats, int seat)
        {
            return -59;
        }
    }
}