using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.API
{
    public interface IAPIHost
    {
        void StartAPIService();

        void CloseAPIService();
    }
}
