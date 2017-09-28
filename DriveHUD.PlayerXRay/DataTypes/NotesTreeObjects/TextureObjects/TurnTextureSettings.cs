namespace AcePokerSolutions.DataTypes.NotesTreeObjects.TextureObjects
{
    public class TurnTextureSettings : TextureSettings
    {
        public bool IsFlushCardFilter { get; set; }
        public bool IsOpenEndedStraightDrawsFilter { get; set; }

        public TurnFlushCardsEnum FlushCard { get; set; }
        public int OpenEndedStraightDraws { get; set; }

        public override bool Equals(object x)
        {
            TextureSettings x1Base = (TextureSettings) x;
            TextureSettings x2Base = this;

            TurnTextureSettings x1 = (TurnTextureSettings) x;
            TurnTextureSettings x2 = this;

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