namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.TextureObjects
{
    public class FlopTextureSettings : TextureSettings
    {
        public FlopTextureSettings()
        {
            FlushCard = FlopFlushCardsEnum.Rainbow;
        }

        public bool IsFlushCardFilter { get; set; }
        public bool IsOpenEndedStraightDrawsFilter { get; set; }

        public FlopFlushCardsEnum FlushCard { get; set; }
        public int OpenEndedStraightDraws { get; set; }

        public override bool Equals(object x)
        {
            TextureSettings x1Base = (TextureSettings) x;
            TextureSettings x2Base = this;

            FlopTextureSettings x1 = (FlopTextureSettings) x;
            FlopTextureSettings x2 = this;

            if (!x1Base.EqualsBase(x2Base))
                goto False;

            if (x1.IsFlushCardFilter != x2.IsFlushCardFilter)
                goto False;

            if (x1.IsFlushCardFilter)
            {
                if (x1.FlushCard != x2.FlushCard)
                    goto False;
            }

            if (x1.IsOpenEndedStraightDrawsFilter != x2.IsOpenEndedStraightDrawsFilter)
                goto False;

            if (x1.IsOpenEndedStraightDrawsFilter)
            {
                if (x1.OpenEndedStraightDraws != x2.OpenEndedStraightDraws)
                    goto False;
            }

            return true;

            False:
            return false;
        }
    }
}