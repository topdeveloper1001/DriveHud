using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace DriveHUD.Application.SplashScreen
{
    internal interface ISplashScreen
    {
        SplashWindowViewModel DataContext { get; set; }

        Dispatcher Dispatcher { get; }

        void CloseSplashScreen();
    }
}
