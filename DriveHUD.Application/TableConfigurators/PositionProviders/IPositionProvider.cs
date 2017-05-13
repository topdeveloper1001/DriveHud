using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    public interface IPositionProvider
    {
        Dictionary<int, int[,]> Positions { get; }
    }
}