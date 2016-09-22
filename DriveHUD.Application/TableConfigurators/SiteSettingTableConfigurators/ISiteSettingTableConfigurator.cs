using Model.Enums;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    internal interface ISiteSettingTableConfigurator
    {
        void ConfigureTable(RadDiagram diagram, SiteModel viewModel, EnumTableType tableType);
    }
}
