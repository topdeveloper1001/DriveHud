using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.Pacific888
{
    internal class Pacific888Importer : IPacific888Importer
    {
        public bool IsRunning
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Site
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler ProcessStopped;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
