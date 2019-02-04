using DriveHUD.Application.ViewModels;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    public interface IFilterTableConfigurator
    {
        void ConfigureTable(RadDiagram diagram, FilterStandardViewModel viewModel, int seats);

        void ConfigureTable(RadDiagram diagram, FilterStandardViewModel viewModel);
    }
}
