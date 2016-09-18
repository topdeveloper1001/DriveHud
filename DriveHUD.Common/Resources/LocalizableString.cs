using System.Globalization;

namespace DriveHUD.Common.Resources
{
    public class LocalizableString : ILocalizableString
    {
        public LocalizableString(string resourceMessageKey, params object[] resourceMessageParams)
        {
            ResourceMessageKey = resourceMessageKey;
            ResourceMessageParams = resourceMessageParams;
        }

        public string ResourceMessageKey { get; private set; }

        public object[] ResourceMessageParams { get; private set; }

        private readonly static CultureInfo defaultCultureInfo = new CultureInfo("en");

        public override string ToString()
        {
            return ToString(defaultCultureInfo);
        }

        public string ToString(CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(ResourceMessageKey))
            {
                return string.Empty;
            }

            string message = CommonResourceManager.Instance.GetResourceString(ResourceMessageKey, cultureInfo ?? defaultCultureInfo);

            if (string.IsNullOrEmpty(message))
            {
                return ResourceMessageKey;
            }

            if (ResourceMessageParams == null || ResourceMessageParams.Length == 0)
            {
                return message;
            }

            return string.Format(message, ResourceMessageParams);
        }
    }
}