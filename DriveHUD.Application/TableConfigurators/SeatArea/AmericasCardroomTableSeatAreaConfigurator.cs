using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.TableConfigurators.SeatArea
{
    internal class AmericasCardroomTableSeatAreaConfigurator : ITableSeatAreaConfigurator
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
                case EnumTableType.Six:
                    resultList = Get6MaxList();
                    break;
                case EnumTableType.Eight:
                    resultList = Get8MaxList();
                    break;
                case EnumTableType.Nine:
                    resultList = Get9MaxList();
                    break;
                default:
                    resultList = new List<ITableSeatArea>();
                    LogProvider.Log.Warn(String.Format("Cannot find predefined ACR table seat areas for next table type: {0}", tableType));
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
                new TableSeatArea(1, 100, 280, 200, 520, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(2, 200, 460, 300, 170, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(3, 100, 280, 200, 20, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
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

        private IEnumerable<ITableSeatArea> Get6MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 137.5, 527.5, 45, 134, "M107.5,1 L206.5,131 471,130.5 571.5,-0.5 z"),
                new TableSeatArea(2, 235.5, 239.833, 49, 550, "M306.48871,17.25673 L302.48871,249.25703 163.48888,249.25703 78.488976,149.2569 186.48885,18.256732 z"),
                new TableSeatArea(3, 213.5, 233.833, 288, 554, "M289.04489,-1.7493764 L290.90316,256.61151 159.895,257.85363 79.060182,129.9153 159.895,-2.9914961 z"),
                new TableSeatArea(4, 108.5, 478.833, 395, 160, "M306.48669,74.01589 L394.48704,208.1657 -64.335173,203.19718 16.069067,75.050585 z"),
                new TableSeatArea(5, 209.5, 219.833, 290, 18, "M205.84095,-1.2508677 L287.33401,129.37623 201.17439,259.59177 68.507274,259.59219 69.840278,-0.42279462 z" ),
                new TableSeatArea(6, 238.833, 229.167, 50, 10, "M190.00019,18.256731 L298.00038,149.7569 216.58357,251.25703 76.99998,251.25703 78.999981,18.256731 z"),
           };
        }

        private IEnumerable<ITableSeatArea> Get8MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 284, 812, 0, 0, "M0,0L0,0 0,0.25 0.5,1 1,0.25 1,0z"),
                new TableSeatArea(2, 210, 406, 75, 406, "M0,1L1,1 1,0z"),
                new TableSeatArea(3, 210, 406, 284, 406, "M0,0L1,1 1,0z"),
                new TableSeatArea(4, 284, 406, 284, 406, "M0,0L0.15,0.3 0.24,1 1,1 1,0.75z"),
                new TableSeatArea(5, 284, 162, 284, 340, "M0,1L1,1 0.77,0.3 0.4,0 0.08,0.31z"),
                new TableSeatArea(6, 284, 406, 284, 0, "M0,0.75L0,1 0.83,1 0.87,0.3 1,0z"),
                new TableSeatArea(7, 210, 406, 284, 0, "M0,0L0,1 1,0z"),
                new TableSeatArea(8, 210, 406, 75, 0, "M0,0L0,1 1,1z"),
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
    }
}
