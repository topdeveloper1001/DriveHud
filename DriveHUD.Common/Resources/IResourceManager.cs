using System.Globalization;

namespace DriveHUD.Common.Resources
{
    public interface IResourceManager
    {       
        string GetResourceString(string resourceKey);

        string GetResourceString(string resourceKey, CultureInfo cultureInfo);
      
        object GetResourceObject(string resourceKey);

        object GetResourceObject(string resourceKey, CultureInfo cultureInfo);
     
        bool Match(string resourceKey);
    }
}