using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DriveHUD.Application.TableConfigurators
{
    internal class BovadaTableSeatAreaConfigurator : ITableSeatAreaConfigurator
    {
        public IEnumerable<ITableSeatArea> GetTableSeatAreas(EnumTableType tableType)
        {
            IEnumerable<ITableSeatArea> resultList;
            switch (tableType)
            {
                case EnumTableType.HU:
                    resultList = GetHUList();
                    break;
                case EnumTableType.Six:
                    resultList = Get6MaxList();
                    break;                
                case EnumTableType.Nine:
                    resultList = Get9MaxList();
                    break;
                default:
                    resultList = new List<ITableSeatArea>();
                    LogProvider.Log.Warn(String.Format("Cannot find predefined bovada table seat areas for next table type: {0}", tableType));
                    break;
            }
            resultList.ForEach(x => x.Initialize());

            return resultList;
        }

        private IEnumerable<ITableSeatArea> GetHUList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 133.833, 514.167, 59, 151, "M107.5,1 L206.5,131 471,130.5 571.5,-0.5 z"),
                new TableSeatArea(2, 109.833, 475.167, 404, 171, "M193.50028,0.5 L104.50034,107.5 574.5,107.5 488.50005,0.5 z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get6MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 137.5, 527.5, 54, 145, "M107.5,1 L206.5,131 471,130.5 571.5,-0.5 z"),
                new TableSeatArea(2, 235.5, 239.833, 58, 561, "M306.48871,17.25673 L302.48871,249.25703 163.48888,249.25703 78.488976,149.2569 186.48885,18.256732 z"),
                new TableSeatArea(3, 213.5, 233.833, 297, 565, "M289.04489,-1.7493764 L290.90316,256.61151 159.895,257.85363 79.060182,129.9153 159.895,-2.9914961 z"),
                new TableSeatArea(4, 108.5, 478.833, 404, 171, "M306.48669,74.01589 L394.48704,208.1657 -64.335173,203.19718 16.069067,75.050585 z"),
                new TableSeatArea(5, 209.5, 219.833, 299, 29, "M205.84095,-1.2508677 L287.33401,129.37623 201.17439,259.59177 68.507274,259.59219 69.840278,-0.42279462 z" ),
                new TableSeatArea(6, 238.833, 229.167, 59, 21, "M190.00019,18.256731 L298.00038,149.7569 216.58357,251.25703 76.99998,251.25703 78.999981,18.256731 z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get9MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 137.5, 285.833, 55, 407, "M268.00025,-22.499654 L161.33392,108.00196 -12.998854,79.001602 -13.999184,-22.999661 z"),
                new TableSeatArea(2, 207.5, 219.0, 60, 589, "M78.166,121 L184.16613,-10.5 272.83235,-9.8333333 278.83267,192.83333 140.1662,193.50001 z"),
                new TableSeatArea(3, 213.5, 171.166, 269, 623, "M106.49372,-6.9990005 L244.9939,-7.4990004 241.9939,201.50102 80.994002,93.501007 z"),
                new TableSeatArea(4, 139.5, 277.166, 374, 511, "M76.665318,109.16691 L184.66508,74.917 349.31574,186.8305 348.66474,214.16668 76.665311,212.16669 z"),
                new TableSeatArea(5, 103.5, 201.833, 409, 305, "M96.999969,109.83227 L299.66628,109.16593 299.66596,213.83276 96.999972,213.16643 z"),
                new TableSeatArea(6, 137.5, 275.833, 372, 24, "M5.0011461,186.49986 L171.00065,73.833335 282.83898,109.33068 283.5053,214.49832 5.0011458,214.49975 z"),
                new TableSeatArea(7, 212.167, 163.833, 269, 23, "M23.994619,-7.1660004 L163.9948,-7.1660004 189.9945,90.834007 23.994619,202.83402 z"),
                new TableSeatArea(8, 199.5, 208.333, 66, 21, "M36.947254,-9.0096328 L135.49987,-7.4996368 239.66645,121.16667 178.29513,192.42086 38.333582,192.49987 z" ),
                new TableSeatArea(9, 135, 275.833, 58, 128, "M242.00003,-23.5 L241.00003,78.5 67.500566,107.00053 -35.665783,-18.49998 z" ),
            };
        }
    }
}
