using System.Resources;
using System.Reflection;
using System.Globalization;

namespace DriveHUD.Common.Resources
{
    public class FileResourceManager : IResourceManager
    {
        private readonly string prefix;
        private readonly ResourceManager resourceManager;
       
        public FileResourceManager(string resourceFileName, Assembly assembly)
            : this(string.Empty, resourceFileName, assembly)
        {
        }
      
        public FileResourceManager(string resourceKeyPrefix, string resourceFileName, Assembly assembly)
        {
            prefix = resourceKeyPrefix;
            resourceManager = new ResourceManager(resourceFileName, assembly);
        }

        private ResourceManager FindResourceManager(string resourceKey)
        {
            return !string.IsNullOrEmpty(resourceKey) && resourceKey.StartsWith(prefix) ? resourceManager : null;
        }

        #region IResourceManager Members
       
        public bool Match(string resourceKey)
        {
            return FindResourceManager(resourceKey) != null;
        }

        private static CultureInfo GetCultureInfo()
        {
            return CultureInfo.CurrentCulture;
        }
     
        public string GetResourceString(string resourceKey)
        {
            return GetResourceString(resourceKey, null);
        }

        public string GetResourceString(string resourceKey, CultureInfo cultureInfo)
        {
            cultureInfo = cultureInfo ?? GetCultureInfo();
            ResourceManager resManager = FindResourceManager(resourceKey);
            return resManager != null ? resManager.GetString(resourceKey, cultureInfo) : string.Empty;
        }

        public object GetResourceObject(string resourceKey)
        {
            return GetResourceObject(resourceKey, null);
        }

        public object GetResourceObject(string resourceKey, CultureInfo cultureInfo)
        {
            cultureInfo = cultureInfo ?? GetCultureInfo();
            ResourceManager resManager = FindResourceManager(resourceKey);
            return resManager != null ? resManager.GetObject(resourceKey, cultureInfo) : null;
        }

        #endregion
    }
}