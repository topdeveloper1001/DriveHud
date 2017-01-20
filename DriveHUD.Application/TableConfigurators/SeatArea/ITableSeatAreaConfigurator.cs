using DriveHUD.Entities;
using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators
{
    internal interface ITableSeatAreaConfigurator
    {
        IEnumerable<ITableSeatArea> GetTableSeatAreas(EnumTableType tableType);
    }
}
