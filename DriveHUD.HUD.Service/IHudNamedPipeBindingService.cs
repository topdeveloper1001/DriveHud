using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DriveHUD.HUD.Service
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IHudNamedPipeBindingCallbackService))]
    public interface IHudNamedPipeBindingService
    {
        [OperationContract(IsOneWay = true)]
        void UpdateHUD(byte[] data);

        #region Call back connection manager

        [OperationContract(IsOneWay = true)]
        void ConnectCallbackChannel(string name);

        #endregion
    }

    public interface IHudNamedPipeBindingCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void Message(string test);
    }
}
