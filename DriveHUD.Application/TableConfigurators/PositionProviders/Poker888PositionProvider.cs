using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    public class Poker888PositionProvider : IPositionProvider
    {
        public Dictionary<int, int[,]> Positions { get; }

        public Poker888PositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {2, new int[,] {{355, 118}, {355, 409}}},
                {3, new int[,] {{636, 262}, {355, 409}, {72, 262}}},
                {4, new int[,] {{355, 118}, {636, 262}, {355, 409}, {72, 262}}},
                {5, new int[,] {{490, 118}, {636, 318}, {355, 409}, {72, 318}, {220, 118}}},
                {6, new int[,] {{422, 118}, {636, 262}, {422, 409}, {264, 409}, {72, 262}, {264, 118}}},
                {
                    8,
                    new int[,]
                        {{422, 118}, {636, 211}, {636, 318}, {422, 409}, {264, 409}, {72, 318}, {72, 211}, {264, 118}}
                },
                {
                    9,
                    new int[,]
                    {
                        {415, 118}, {636, 211}, {636, 318}, {490, 409}, {355, 409}, {220, 409}, {72, 318}, {72, 211},
                        {273, 118}
                    }
                },
                {
                    10,
                    new int[,]
                    {
                        {490, 118}, {636, 211}, {636, 318}, {490, 409}, {355, 409}, {220, 409}, {72, 318}, {72, 211},
                        {220, 118}, {355, 118}
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