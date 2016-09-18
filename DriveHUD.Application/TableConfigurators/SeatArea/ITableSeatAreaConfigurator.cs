using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.TableConfigurators
{
    internal interface ITableSeatAreaConfigurator
    {
        IEnumerable<ITableSeatArea> GetTableSeatAreas(EnumTableType tableType);
    }
}
