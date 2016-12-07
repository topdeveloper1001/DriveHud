using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.HUD.Services
{
    interface IHudServiceHost : IDisposable
    {
        void Initialize();

        void OpenHost();
    }
}
