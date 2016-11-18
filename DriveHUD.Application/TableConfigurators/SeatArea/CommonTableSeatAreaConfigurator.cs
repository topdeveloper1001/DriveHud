using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;

namespace DriveHUD.Application.TableConfigurators
{
    internal class CommonTableSeatAreaConfigurator : ITableSeatAreaConfigurator
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
                new TableSeatArea(1, 100, 460, 10, 70, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(2, 100, 460, 200, 70, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get3MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 100, 460, 10, 70, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(2, 100, 270, 200, 310, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(3, 100, 280, 200, 20, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get4MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 140, 280, 10, 310, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(2, 140, 280, 160, 310, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(3, 140, 280, 160, 20, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(4, 140, 280, 10, 20, "M55,90.5 L-505,90.5 -505,195 55,195 z"),

            };
        }

        private IEnumerable<ITableSeatArea> Get5MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 140, 280, 10, 310, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(2, 140, 280, 160, 310, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(3, 140, 280, 160, 20, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(4, 140, 280, 10, 20, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
                new TableSeatArea(5, 140, 280, 10, 20, "M55,90.5 L-505,90.5 -505,195 55,195 z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get6MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 142.989, 171.1, 10, 410, "M306.48871,17.25673 L302.48871,249.25703 163.48888,249.25703 78.488976,149.2569 186.48885,18.256732 z"),
                new TableSeatArea(2, 132.489, 165.1, 160, 410, "M289.04489,-1.7493764 L290.90316,256.61151 159.895,257.85363 79.060182,129.9153 159.895,-2.9914961 z"),
                new TableSeatArea(3, 96.489, 340.6, 210, 120, "M306.48669,74.01589 L394.48704,208.1657 -64.335173,203.19718 16.069067,75.050585 z"),
                new TableSeatArea(4, 120.489, 150.1, 160, 20, "M205.84095,-1.2508677 L287.33401,129.37623 201.17439,259.59177 68.507274,259.59219 69.840278,-0.42279462 z" ),
                new TableSeatArea(5, 142.989, 147.1, 10, 20, "M190.00019,18.256731 L298.00038,149.7569 216.58357,251.25703 76.99998,251.25703 78.999981,18.256731 z"),
                new TableSeatArea(6, 96.489, 346.6, 20, 120, "M107.5,1 L206.5,131 471,130.5 571.5,-0.5 z"),
            };
        }

        private IEnumerable<ITableSeatArea> Get8MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 135, 142.198, 25, 465, "M82.755717,98.204265 L-57.736756,181.36113 -59.755354,232.72292 78.718524,235.85385 z"),
                new TableSeatArea(2, 126, 137.495, 160, 470, "M82.755717,98.204265 L-55.008314,100.65982 -24.229796,223.52703 75.690622,226.60873 z"),
                new TableSeatArea(3, 119.203, 228.491, 205, 375, "M37.340888,129.84007 L-51.976873,130.24116 -49.462224,251.2624 173.5928,245.09899 173.5928,221.47257 67.12787,216.51024 z"),
                new TableSeatArea(4, 123.692, 170.675, 205, 205, "M73.70546,126.8356 L-92.533618,124.20168 -90.843046,248.18068 78.718515,250.23515 z"),
                new TableSeatArea(5, 124, 175, 205, 24, "M77.709218,125.93976 L53.486139,124.91253 -20.192397,210.173 -97.908112,215.30918 -98.917407,248.18069 78.718515,250.23515 z"),
                new TableSeatArea(6, 125, 135, 165, 15, "M142.3041,149.56616 L139.27621,85.877611 7.058567,86.904842 12.105042,209.14577 89.820754,207.0913 z"),
                new TableSeatArea(7, 97, 145, 65, 5, "M142.30409,179.35596 L52.47684,105.39507 -3.0343826,104.36783 -2.0250876,199.90066 140.2855,202.98236 z"),
                new TableSeatArea(8, 106, 189, 5, 15, "M157.44352,113.61295 L-29.276052,114.64018 -29.276052,167.02915 8.067862,167.02915 82.788479,220.46709 160.4714,221.47259 z" )
            };
        }

        private IEnumerable<ITableSeatArea> Get9MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 135, 142.198, 25, 465, "M82.755717,98.204265 L-57.736756,181.36113 -59.755354,232.72292 78.718524,235.85385 z"),
                new TableSeatArea(2, 126, 137.495, 160, 470, "M82.755717,98.204265 L-55.008314,100.65982 -24.229796,223.52703 75.690622,226.60873 z"),
                new TableSeatArea(3, 119.203, 228.491, 205, 375, "M37.340888,129.84007 L-51.976873,130.24116 -49.462224,251.2624 173.5928,245.09899 173.5928,221.47257 67.12787,216.51024 z"),
                new TableSeatArea(4, 123.692, 170.675, 205, 205, "M73.70546,126.8356 L-92.533618,124.20168 -90.843046,248.18068 78.718515,250.23515 z"),
                new TableSeatArea(5, 124, 175, 205, 24, "M77.709218,125.93976 L53.486139,124.91253 -20.192397,210.173 -97.908112,215.30918 -98.917407,248.18069 78.718515,250.23515 z"),
                new TableSeatArea(6, 125, 135, 165, 15, "M142.3041,149.56616 L139.27621,85.877611 7.058567,86.904842 12.105042,209.14577 89.820754,207.0913 z"),
                new TableSeatArea(7, 97, 145, 65, 5, "M142.30409,179.35596 L52.47684,105.39507 -3.0343826,104.36783 -2.0250876,199.90066 140.2855,202.98236 z"),
                new TableSeatArea(8, 106, 189, 5, 15, "M157.44352,113.61295 L-29.276052,114.64018 -29.276052,167.02915 8.067862,167.02915 82.788479,220.46709 160.4714,221.47259 z" ),
                new TableSeatArea(9, 113.668, 170.675, 5, 205, "M73.70546,126.8356 L-92.533618,124.20168 -90.84276,237.88388 78.718801,239.93835 z" )
            };
        }

        private IEnumerable<ITableSeatArea> Get10MaxList()
        {
            return new List<ITableSeatArea>()
            {
                new TableSeatArea(1, 135, 142.198, 25, 465, "M82.755717,98.204265 L-57.736756,181.36113 -59.755354,232.72292 78.718524,235.85385 z"),
                new TableSeatArea(2, 126, 137.495, 160, 470, "M82.755717,98.204265 L-55.008314,100.65982 -24.229796,223.52703 75.690622,226.60873 z"),
                new TableSeatArea(3, 119.203, 228.491, 205, 375, "M37.340888,129.84007 L-51.976873,130.24116 -49.462224,251.2624 173.5928,245.09899 173.5928,221.47257 67.12787,216.51024 z"),
                new TableSeatArea(4, 123.692, 170.675, 205, 205, "M73.70546,126.8356 L-92.533618,124.20168 -90.843046,248.18068 78.718515,250.23515 z"),
                new TableSeatArea(5, 124, 175, 205, 24, "M77.709218,125.93976 L53.486139,124.91253 -20.192397,210.173 -97.908112,215.30918 -98.917407,248.18069 78.718515,250.23515 z"),
                new TableSeatArea(6, 125, 135, 165, 15, "M142.3041,149.56616 L139.27621,85.877611 7.058567,86.904842 12.105042,209.14577 89.820754,207.0913 z"),
                new TableSeatArea(7, 97, 145, 65, 5, "M142.30409,179.35596 L52.47684,105.39507 -3.0343826,104.36783 -2.0250876,199.90066 140.2855,202.98236 z"),
                new TableSeatArea(8, 106, 189, 5, 15, "M157.44352,113.61295 L-29.276052,114.64018 -29.276052,167.02915 8.067862,167.02915 82.788479,220.46709 160.4714,221.47259 z" ),
                new TableSeatArea(9, 113.668, 170.675, 5, 205, "M73.70546,126.8356 L-92.533618,124.20168 -90.84276,237.88388 78.718801,239.93835 z" ),
                new TableSeatArea(10, 118.813, 229.491, 5, 375, "M178.63931,131.0763 L-51.976873,130.24116 -49.462224,251.2624 33.299975,250.23517 43.392984,222.49992 177.63001,152.64817 z" ), };
        }
    }
}
