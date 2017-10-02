using System;
using System.IO;
using System.Reflection;

namespace AcePokerSolutions.Helpers
{
    public static class AssemblyHelpers
    {
        #region Public Methods

        public static string GetRunningPath(Assembly assembly)
        {
            return String.Format(@"{0}\", Path.GetDirectoryName(assembly.Location));
        }

        public static string GetShortApplicationVersion(Assembly assembly)
        {
            string version = GetVersion(assembly).ToString();
            string[] parts = version.Split('.');
            return string.Format("{0}.{1}.{2}", parts[0], parts[1], parts[2]);
        }

        public static string GetStringVersion(Assembly assembly)
        {
            return assembly.GetName().Version.ToString();
        }

        public static string DateCompiled(Assembly assembly)
        {
            Version v = GetVersion(assembly);

            DateTime t = new DateTime(
                v.Build * TimeSpan.TicksPerDay +
                v.Revision * TimeSpan.TicksPerSecond * 2
                ).AddYears(1999);

            return t.ToShortDateString();
        }

        #endregion

        #region Private Methods

        private static Version GetVersion(Assembly assembly)
        {
            return assembly.GetName().Version;
        }

        #endregion
    }
}
