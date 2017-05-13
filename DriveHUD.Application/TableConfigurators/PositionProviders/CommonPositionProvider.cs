using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    public class CommonPositionProvider : IPositionProvider
    {
        public CommonPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {2, new[,] {{347, 133}, {347, 418}}},
                {3, new[,] {{352, 105}, {541, 422}, {161, 422}}},
                {4, new[,] {{340, 143}, {657, 291}, {360, 442}, {41, 286}}},
                {5, new[,] {{352, 105}, {660, 256}, {352, 411}, {57, 256}, {0, 0}}},
                // test values
                { 6, new[,] { { 324, 64 }, { 608, 135 }, { 626, 391 }, { 343, 413 }, { 78, 377 }, { 79, 218 } } },
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

            PlayerLabelHeight = 52;
        }

        public Dictionary<int, int[,]> Positions { get; }

        public int PlayerLabelHeight { get; }
    }
}