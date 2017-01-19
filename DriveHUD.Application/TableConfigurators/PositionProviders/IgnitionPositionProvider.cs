using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    public class IgnitionPositionProvider : IPositionProvider
    {
        public Dictionary<int, int[,]> Positions { get; }

        public IgnitionPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {2, new[,] {{338, 142}, {342, 484}}},
                {6, new[,] {{338, 142}, {639, 229}, {639, 415}, {342, 494}, {47, 416}, {38, 227}}},
                {
                    9,
                    new[,]
                    {
                        {455, 142}, {639, 213}, {648, 362}, {524, 481}, {340, 493}, {156, 481}, {29, 361}, {41, 214},
                        {225, 146}
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
            return 35;
        }
    }

    public class IgnitionRichPositionProvider : IPositionProvider
    {
        public Dictionary<int, int[,]> Positions { get; }

        public IgnitionRichPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 334, 45 }, { 334, 389 } } },
                { 6, new int[,] { { 334, 45 }, { 631, 131 }, { 631, 319 }, { 334, 399 }, { 36, 319 }, { 36, 131 } } },
                { 9, new int[,] { { 451, 45 }, { 632, 116 }, { 645, 263 }, { 524, 384 }, { 333, 398 }, { 148, 384 }, { 22, 263 }, { 38, 116 }, { 218, 45 } } }
            };
        }

        private bool IsRightOriented(int seats, int seat)
        {
            return (seats > 6 && seat < 5) || (seats < 7 && seats > 2 && seat < 3) || (seats < 3 && seat < 1);
        }

        public int GetOffsetX(int seats, int seat)
        {
            return IsRightOriented(seats, seat) ? 0 : -30;
        }

        public int GetOffsetY(int seats, int seat)
        {
            return -60;
        }
    }
}