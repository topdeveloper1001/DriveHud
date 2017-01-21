using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    public interface IPositionProvider
    {
        Dictionary<int, int[,]> Positions { get; }
        int GetOffsetX(int seats, int seat);
        int GetOffsetY(int seats, int seat);
    }
}