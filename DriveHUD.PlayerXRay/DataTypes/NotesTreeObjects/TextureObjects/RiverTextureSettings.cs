﻿namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.TextureObjects
{
    public class RiverTextureSettings : TextureSettings
    {
        public bool IsFlushCardFilter { get; set; }
        public RiverFlushCardsEnum FlushCard { get; set; }

        public override bool Equals(object x)
        {
            TextureSettings x1Base = (TextureSettings) x;
            TextureSettings x2Base = this;

            RiverTextureSettings x1 = (RiverTextureSettings) x;
            RiverTextureSettings x2 = this;

            if (!x1Base.EqualsBase(x2Base))
                goto False;

            if (x1.IsFlushCardFilter != x2.IsFlushCardFilter)
                goto False;
            if (x1.IsFlushCardFilter)
            {
                if (x1.FlushCard != x2.FlushCard)
                    goto False;
            }

            return true;

            False:
            return false;
        }
    }
}