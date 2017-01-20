using DriveHUD.Application.ViewModels.Settings;
using DriveHUD.Entities;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    internal interface ISiteSettingTableConfigurator
    {
        void ConfigureTable(RadDiagram diagram, SettingsSiteViewModel viewModel, EnumTableType tableType);
    }
}
