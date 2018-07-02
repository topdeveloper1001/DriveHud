using DriveHUD.Entities;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Simulator
{
    public interface IPokerEmulator : INotifyPropertyChanged
    {
        string Name { get; }

        EnumPokerSites Site { get; }

        bool CanRun { get; }

        Task Run(CancellationTokenSource cancellationToken);
    }
}