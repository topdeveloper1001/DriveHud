using ProtoBuf;

namespace DriveHUD.Importers.PokerKing.Model
{
    [ProtoContract]
    internal class RequestLogon
    {
        [ProtoMember(1)]
        public string Version { get; set; }

        [ProtoMember(2)]
        public string Token { get; set; }

        [ProtoMember(3)]
        public PositionInfo Position { get; set; }

        [ProtoMember(4)]
        public string DeviceInfo { get; set; }  
    }
}