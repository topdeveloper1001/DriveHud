using DriveHUD.Common.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Common.Utils
{
    public static class FileHelper
    {
        /// <summary>
        /// Checks if it is possible to write to the directory specified
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="throwIfFails"></param>
        /// <returns></returns>
        public static bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                { }
                return true;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error("", $"Can't write to the directory {dirPath}", ex);

                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }
    }
}
