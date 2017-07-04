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
                {2, new int[,] { {580, 320}, {80, 320}}},
                {3, new int[,] {{ 440, 52 }, { 325, 345 }, { 218, 52 } }},
                {4, new int[,] {{ 440, 52 }, { 416, 345 }, { 245, 345 }, { 218, 52 } }},
                {6, new int[,] {{440, 52}, {660, 195}, {416, 345}, {245, 345}, {9, 195}, {218, 52}}},
                {
                    8,
                    new int[,]
                        {{440, 52}, {580, 80}, {580, 320}, {416, 345}, {245, 345}, {80, 320}, {80 , 80}, {218, 52}}
                }, 
                {
                    9,
                    new int[,]
                    {
                        {440, 52}, {580, 80}, {660, 195},
                        {530, 320}, {325, 345},
                        {120, 320}, {9, 195}, {80 , 80},
                        {218, 52}
                    }
                },
                {
                    10,
                    new int[,]
                        {{440, 52}, {580, 80}, {660, 195}, 
                        {580, 320}, {416, 345}, {245, 345}, 
                        {80, 320}, {9, 195}, {80 , 80}, 
                        {218, 52}}
                }
            };

            PlayerLabelWidth = 150;
            PlayerLabelHeight = 45;
        }
    }
}
