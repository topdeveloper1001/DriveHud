namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects
{
    public class PlayerObject
    {
        public PlayerObject()
        {
            Include = true;
        }

        public PlayerObject(PlayerTypeEnum type) : this()
        {
            PlayerType = type;

            switch (type)
            {
                case PlayerTypeEnum.Fish:
                    VpIpMin = 38;
                    VpIpMax = 55;
                    PfrMin = 0;
                    PfrMax = 15;
                    AggMin = 0;
                    AggMax = 3;
                    ThreeBetMin = 0;
                    ThreeBetMax = 5;
                    WtsdMin = 18;
                    WtsdMax = 38;
                    WsdMin = 0;
                    WsdMax = 55;
                    WwsfMin = 0;
                    WwsfMax = 40;
                    break;
                case PlayerTypeEnum.Gambler:
                    VpIpMin = 25;
                    VpIpMax = 40;
                    PfrMin = 15;
                    PfrMax = 30;
                    AggMin = 2.5;
                    AggMax = 8;
                    ThreeBetMin = 5;
                    ThreeBetMax = 18;
                    WtsdMin = 21;
                    WtsdMax = 26;
                    WsdMin = 48;
                    WsdMax = 57;
                    WwsfMin = 39;
                    WwsfMax = 48;
                    break;
                case PlayerTypeEnum.Lag:
                    VpIpMin = 20;
                    VpIpMax = 25;
                    PfrMin = 17;
                    PfrMax = 23;
                    AggMin = 2.4;
                    AggMax = 4;
                    ThreeBetMin = 5;
                    ThreeBetMax = 8.8;
                    WtsdMin = 21;
                    WtsdMax = 26;
                    WsdMin = 48;
                    WsdMax = 57;
                    WwsfMin = 39;
                    WwsfMax = 48;
                    break;
                case PlayerTypeEnum.Nit:
                    VpIpMin = 20;
                    VpIpMax = 25;
                    PfrMin = 17;
                    PfrMax = 23;
                    AggMin = 2.4;
                    AggMax = 4;
                    ThreeBetMin = 5;
                    ThreeBetMax = 8.8;
                    WtsdMin = 21;
                    WtsdMax = 26;
                    WsdMin = 48;
                    WsdMax = 57;
                    WwsfMin = 39;
                    WwsfMax = 48;
                    break;
                case PlayerTypeEnum.Rock:
                    VpIpMin = 20;
                    VpIpMax = 25;
                    PfrMin = 17;
                    PfrMax = 23;
                    AggMin = 2.4;
                    AggMax = 4;
                    ThreeBetMin = 5;
                    ThreeBetMax = 8.8;
                    WtsdMin = 21;
                    WtsdMax = 26;
                    WsdMin = 48;
                    WsdMax = 57;
                    WwsfMin = 39;
                    WwsfMax = 48;
                    break;
                case PlayerTypeEnum.Tag:
                    VpIpMin = 20;
                    VpIpMax = 25;
                    PfrMin = 17;
                    PfrMax = 23;
                    AggMin = 2.4;
                    AggMax = 4;
                    ThreeBetMin = 5;
                    ThreeBetMax = 8.8;
                    WtsdMin = 21;
                    WtsdMax = 26;
                    WsdMin = 48;
                    WsdMax = 57;
                    WwsfMin = 39;
                    WwsfMax = 48;
                    break;
                case PlayerTypeEnum.Whale:
                    VpIpMin = 20;
                    VpIpMax = 25;
                    PfrMin = 17;
                    PfrMax = 23;
                    AggMin = 2.4;
                    AggMax = 4;
                    ThreeBetMin = 5;
                    ThreeBetMax = 8.8;
                    WtsdMin = 21;
                    WtsdMax = 26;
                    WsdMin = 48;
                    WsdMax = 57;
                    WwsfMin = 39;
                    WwsfMax = 48;
                    break;
            }
        }

        public bool Include { get; set; }
        public PlayerTypeEnum PlayerType { get; set; }

        public double VpIpMin { get; set; }
        public double VpIpMax { get; set; }

        public double PfrMin { get; set; }
        public double PfrMax { get; set; }

        public double AggMin { get; set; }
        public double AggMax { get; set; }

        public double ThreeBetMin { get; set; }
        public double ThreeBetMax { get; set; }

        public double WtsdMin { get; set; }
        public double WtsdMax { get; set; }

        public double WsdMin { get; set; }
        public double WsdMax { get; set; }

        public double WwsfMin { get; set; }
        public double WwsfMax { get; set; }

        public override bool Equals(object obj)
        {
            PlayerObject p = (PlayerObject) obj;

            return p.Include == Include &&
                   p.AggMax == AggMax &&
                   p.AggMin == AggMin &&
                   p.PfrMax == PfrMax &&
                   p.PfrMin == PfrMin &&
                   p.ThreeBetMax == ThreeBetMax &&
                   p.ThreeBetMin == ThreeBetMin &&
                   p.VpIpMax == VpIpMax &&
                   p.VpIpMin == VpIpMin &&
                   p.WsdMax == WsdMax &&
                   p.WsdMin == WsdMin &&
                   p.WtsdMax == WtsdMax &&
                   p.WtsdMin == WtsdMin &&
                   p.WwsfMax == WwsfMax &&
                   p.WwsfMin == WwsfMin;
        }
    }
}