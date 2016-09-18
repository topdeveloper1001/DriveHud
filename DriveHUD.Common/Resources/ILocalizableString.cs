using System.Globalization;

namespace DriveHUD.Common.Resources
{
    public interface ILocalizableString
    {
        string ToString(CultureInfo cultureInfo);
    }
}