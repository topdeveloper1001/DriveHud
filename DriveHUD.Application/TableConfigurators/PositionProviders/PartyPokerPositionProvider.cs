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

        public int PlayerLabelHeight
        {
            get;
        }

        public int PlayerLabelWidth
        {
            get;
        }

        public PartyPokerPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {2, new int[,] { { 580, 285}, {80, 285}}},
                {3, new int[,] {{ 440, 17 }, { 325, 310 }, { 218, 17 } }},
                {4, new int[,] {{ 440, 17 }, { 416, 310 }, { 245, 310 }, { 218, 17 } }},
                {6, new int[,] {{440, 17}, {660, 160}, {416, 310}, {245, 310}, {9, 160}, {218, 17}}},
                {
                    8,
                    new int[,]
                        {{440, 17}, {580, 45}, {580, 285}, {416, 310}, {245, 310}, {80, 285}, {80 , 45}, {218, 17}}
                },
                {
                    9,
                    new int[,]
                    {
                        {440, 17}, {580, 45}, {660, 160},
                        {530, 285}, {325, 310},
                        {120, 285}, {9, 160}, {80 , 45},
                        {218, 17}
                    }
                },
                {
                    10,
                    new int[,]
                        {{440, 17}, {580, 45}, {660, 160},
                        {580, 285}, {416, 310}, {245, 310},
                        {80, 285}, {9, 160}, {80 , 45},
                        {218, 17}}
                }
            };

            PlayerLabelWidth = 120;
            PlayerLabelHeight = 80;
        }
    }
}
