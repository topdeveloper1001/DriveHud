using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DriveHUD.Common.Resources
{
    public class CompositeKeyProvider : IResXKeyProvider
    {
        private const string NoKey = "<nokey>";

        public string BaseKey { get; set; }

        public CompositeKeyProvider()
        {
            BaseKey = NoKey;
        }

        public CompositeKeyProvider(string baseKey)
        {
            BaseKey = baseKey ?? NoKey;
        }

        public static string GetKey(string prefix, IEnumerable<object> parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return prefix;
            }

            var stringBuilder = new StringBuilder(prefix);

            foreach (object parameter in parameters)
            {
                if (parameter is CultureInfo)
                {
                    continue;
                }

                if (parameter != null)
                {
                    stringBuilder.AppendFormat("_{0}_{1}", parameter.GetType().Name, parameter);
                }
            }

            return stringBuilder.ToString();
        }

        public string ProvideKey(IEnumerable<object> parameters)
        {
            return GetKey(BaseKey, parameters);
        }

        public static string GetEnumKey(Enum enumValue)
        {
            return GetKey("Enum", new object[] { enumValue });
        }
    }
}