//-----------------------------------------------------------------------
// <copyright file="Poker888TableSeatAreaConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using Model.Enums;
using System;
using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators
{
    internal class Poker888TableSeatAreaConfigurator : ITableSeatAreaConfigurator
    {
        public IEnumerable<ITableSeatArea> GetTableSeatAreas(EnumTableType tableType)
        {
            IEnumerable<ITableSeatArea> resultList;
            switch (tableType)
            {
                case EnumTableType.HU:
                    resultList = GetHUList();
                    break;
                case EnumTableType.Three:
                    resultList = Get3MaxList();
                    break;
                case EnumTableType.Four:
                    resultList = Get4MaxList();
                    break;
                case EnumTableType.Five:
                    resultList = Get5MaxList();
                    break;
                case EnumTableType.Six:
                    resultList = Get6MaxList();
                    break;
                case EnumTableType.Eight:
                    resultList = Get8MaxList();
                    break;
                case EnumTableType.Nine:
                    resultList = Get9MaxList();
                    break;
                case EnumTableType.Ten:
                    resultList = Get10MaxList();
                    break;
                default:
                    resultList = new List<ITableSeatArea>();
                    LogProvider.Log.Warn(String.Format("Cannot find predefined 888 table seat areas for next table type: {0}", tableType));
                    break;
            }
            resultList.ForEach(x => x.Initialize());

            return resultList;
        }

        private IEnumerable<ITableSeatArea> GetHUList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 200, 460, 65, 170, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(2, 200, 460, 300, 170, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get3MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 150, 280, 200, 520, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(2, 150, 460, 350, 170, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(3, 150, 280, 200, 20, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get4MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 284, 812, 0, 0, "M0,0L0,0 0.5,1 1,0z"),
                new TableSeatArea(2, 568, 406, 0, 406, "M1,0L0,0.5 1,1z"),
                new TableSeatArea(3, 284, 812, 284, 0, "M0,1L0.5,0 1,1z"),
                new TableSeatArea(4, 568, 406, 0, 0, "M0,0L1,0.5 0,1z"),

            };
        }

        private IEnumerable<ITableSeatArea> Get5MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 284, 406, 0, 406, "M0,0L0,1 1,0.7 1,0z"),
                new TableSeatArea(2, 368, 406, 200, 406, "M0,0.22L0.75,1 1,1 1,0z"),
                new TableSeatArea(3, 284, 600, 284, 108, "M0,1L0.5,0 1,1z"),
                new TableSeatArea(4, 368, 406, 200, 0, "M0,0L0,1 0.26,1 1,0.22z"),
                new TableSeatArea(5, 284, 406, 0, 0, "M0,0L0,0.7 1,1 1,0z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get6MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 284, 406, 0, 406, "M0,0L0,1 1,0.4 1,0z"),
                new TableSeatArea(2, 330, 406, 120, 406, "M0,0.5L1,1 1,0z"),
                new TableSeatArea(3, 284, 406, 284, 406, "M0,0L0,1 1,1 1,0.6z"),
                new TableSeatArea(4, 284, 406, 284, 0, "M0,0.6L0,1 1,1 1,0z" ),
                new TableSeatArea(5, 330, 406, 120, 0, "M0,0L1,0.5 0,1z"),
                new TableSeatArea(6, 284, 406, 0, 0, "M0,0L0,0.4 1,1 1,0z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get8MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 284, 406, 0, 406, "M0,0L0,1 1,0.25 1,0z"),
                new TableSeatArea(2, 210, 406, 75, 406, "M0,1L1,1 1,0z"),
                new TableSeatArea(3, 210, 406, 284, 406, "M0,0L1,1 1,0z"),
                new TableSeatArea(4, 284, 406, 284, 406, "M0,0L0,1 1,1 1,0.75z"),
                new TableSeatArea(5, 284, 406, 284, 0, "M0,0.75L0,1 1,1 1,0z"),
                new TableSeatArea(6, 210, 406, 284, 0, "M0,0L0,1 1,0z"),
                new TableSeatArea(7, 210, 406, 75, 0, "M0,0L0,1 1,1z"),
                new TableSeatArea(8, 284, 406, 0, 0, "M0,0L0,0.25 1,1 1,0z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get9MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 284, 406, 0, 406, "M0,0L0,1 1,0.25 1,0z"),
                new TableSeatArea(2, 210, 406, 75, 406, "M0,1L1,1 1,0z"),
                new TableSeatArea(3, 210, 406, 284, 406, "M0,0L1,1 1,0z"),
                new TableSeatArea(4, 284, 406, 284, 406, "M0,0L0.15,0.3 0.24,1 1,1 1,0.75z"),
                new TableSeatArea(5, 284, 162, 284, 340, "M0,1L1,1 0.77,0.3 0.4,0 0.08,0.31z"),
                new TableSeatArea(6, 284, 406, 284, 0, "M0,0.75L0,1 0.83,1 0.87,0.3 1,0z"),
                new TableSeatArea(7, 210, 406, 284, 0, "M0,0L0,1 1,0z"),
                new TableSeatArea(8, 210, 406, 75, 0, "M0,0L0,1 1,1z"),
                new TableSeatArea(9, 284, 406, 0, 0, "M0,0L0,0.25 1,1 1,0z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get10MaxList()
        {
            return new List<ITableSeatArea>()
            {
               new TableSeatArea(1, 284, 406, 0, 406, "M0.24,0L0.15,0.7 0,1 1,0.25 1,0z"),
                new TableSeatArea(2, 210, 406, 75, 406, "M0,1L1,1 1,0z"),
                new TableSeatArea(3, 210, 406, 284, 406, "M0,0L1,1 1,0z"),
                new TableSeatArea(4, 284, 406, 284, 406, "M0,0L0.15,0.3 0.24,1 1,1 1,0.75z"),
                new TableSeatArea(5, 284, 162, 284, 340, "M0,1L1,1 0.77,0.3 0.4,0 0.08,0.31z"),
                new TableSeatArea(6, 284, 406, 284, 0, "M0,0.75L0,1 0.83,1 0.87,0.3 1,0z"),
                new TableSeatArea(7, 210, 406, 284, 0, "M0,0L0,1 1,0z"),
                new TableSeatArea(8, 210, 406, 75, 0, "M0,0L0,1 1,1z"),
                new TableSeatArea(9, 284, 406, 0, 0, "M0,0L0,0.25 1,1 0.87,0.7 0.83,0z"),
                new TableSeatArea(10, 284, 162, 0, 340, "M0,0L0.08,0.69 0.4,1 0.77,0.7 1,0z")
            };
        }
    }
}
