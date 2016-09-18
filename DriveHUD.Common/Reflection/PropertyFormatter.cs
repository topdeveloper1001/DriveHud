using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Common.Reflection
{
    public class PropertyFormatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) return string.Empty;
            object memberValue = ReflectionHelper.GetMemberValue(arg, format);
            return memberValue == null ? string.Empty : memberValue.ToString();
        }
    }
}
