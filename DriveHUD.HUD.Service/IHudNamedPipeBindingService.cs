using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows;

namespace DriveHUD.HUD.Service
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IHudNamedPipeBindingCallbackService))]
    public interface IHudNamedPipeBindingService
    {
        [OperationContract(IsOneWay = true)]
        void UpdateHUD(byte[] data);

        #region Callback connection manager

        [OperationContract(IsOneWay = true)]
        void ConnectCallbackChannel(string name);

        #endregion
    }

    public interface IHudNamedPipeBindingCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void SaveHudLayout(HudLayoutContract hudLayout);

        [OperationContract(IsOneWay = true)]
        void ReplayHand(long gameNumber, short pokerSiteId);

        [OperationContract(IsOneWay = true)]
        void LoadLayout(int layoutId, string layoutName);
    }

    [DataContract]
    public class HudLayoutContract
    {
        [DataMember]
        public int LayoutId { get; set; }

        [DataMember]
        public List<HudPositionContract> HudPositions { get; set; }
    }

    [DataContract]
    public class HudPositionContract
    {
        [DataMember]
        public int SeatNumber { get; set; }

        [DataMember]
        public Point Position { get; set; }

        [DataMember]
        public int HudType { get; set; }
    }

}
