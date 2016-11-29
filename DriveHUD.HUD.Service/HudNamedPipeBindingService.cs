using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DriveHUD.HUD.Service
{
    public class HudNamedPipeBindingService : IHudNamedPipeBindingService
    {
        public void ConnectCallbackChannel()
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void UpdateHUD(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
