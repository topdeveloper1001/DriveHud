using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Bootstrapper.App
{
    public interface ICancelable
    {
        void Cancel(object obj);
    }
}
