using System.Collections.Generic;

namespace DriveHUD.Common.Resources
{
    public interface IResXKeyProvider
    {
        string ProvideKey(IEnumerable<object> parameters);
    }
}