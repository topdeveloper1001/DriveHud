//-----------------------------------------------------------------------
// <copyright file="RiverTextureSettings.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ReactiveUI;

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.TextureObjects
{
    public class RiverTextureSettings : TextureSettings
    {       
        private RiverFlushCardsEnum flushCard;

        public RiverFlushCardsEnum FlushCard
        {
            get
            {
                return flushCard;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref flushCard, value);
            }
        }

        public override NoteStageType StageType
        {
            get
            {
                return NoteStageType.River;
            }
        }

        public override bool Equals(object obj)
        {
            var riverTextureSettings = obj as RiverTextureSettings;

            if (riverTextureSettings == null)
            {
                return false;
            }

            return base.Equals(obj) && (IsFlushCardFilter && FlushCard == riverTextureSettings.FlushCard || !IsFlushCardFilter);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = base.GetHashCode();
                hash += hash * 31 + FlushCard.GetHashCode();

                return hash;
            }
        }
    }
}