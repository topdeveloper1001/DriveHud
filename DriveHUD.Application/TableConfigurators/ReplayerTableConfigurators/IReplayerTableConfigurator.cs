
using Telerik.Windows.Controls;
using DriveHUD.Application.ViewModels.Replayer;

namespace DriveHUD.Application.TableConfigurators
{
    public interface IReplayerTableConfigurator
    {
        void ConfigureTable(RadDiagram diagram, ReplayerViewModel viewModel);
    }
}