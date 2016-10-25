using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers
{
    internal interface IImporterInternalService : IImporterService
    {
        /// <summary>
        /// Register importer
        /// </summary>
        /// <typeparam name="T">Importer interface</typeparam>
        IImporterService Register<T>() where T : IBackgroundProcess;
    }
}