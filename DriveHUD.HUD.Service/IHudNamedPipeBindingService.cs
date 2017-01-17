using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Windows;

namespace DriveHUD.HUD.Service
{
    [ServiceContract(Name = "IHudNamedPipeBindingService", SessionMode = SessionMode.Required, CallbackContract = typeof(IHudNamedPipeBindingCallbackService))]
    public interface IHudNamedPipeBindingService
    {
        [OperationContract(Name = "UpdateHUD", IsOneWay = true)]
        void UpdateHUD(byte[] data);

        #region Callback connection manager

        [OperationContract(Name = "ConnectCallbackChannel", IsOneWay = true)]
        void ConnectCallbackChannel(string name);

        #endregion
    }

    public interface IHudNamedPipeBindingCallbackService
    {
        [OperationContract(Name = "SaveHudLayout", IsOneWay = true)]
        void SaveHudLayout(HudLayoutContract hudLayout, short pokerSiteId, short gameType, short tableType);

        [OperationContract(Name = "ReplayHand", IsOneWay = true)]
        void ReplayHand(long gameNumber, short pokerSiteId);

        [OperationContract(Name = "LoadLayout", IsOneWay = true)]
        void LoadLayout(string layoutName, short pokerSiteId, short gameType, short tableType);

        [OperationContract(Name = "TagHand", IsOneWay = true)]
        void TagHand(long gameNumber, short pokerSiteId, int tag);
    }

    [DataContract]
    public class HudLayoutContract
    {
        [DataMember(Name = "LayoutName")]
        public string LayoutName { get; set; }

        [DataMember(Name = "HudPositions")]
        public List<HudPositionContract> HudPositions { get; set; }
    }

    [DataContract]
    public class HudPositionContract
    {
        [DataMember(Name = "SeatNumber")]
        public int SeatNumber { get; set; }

        [DataMember(Name = "Position")]
        public Point Position { get; set; }

        [DataMember(Name = "HudType")]
        public int HudType { get; set; }
    }

}
