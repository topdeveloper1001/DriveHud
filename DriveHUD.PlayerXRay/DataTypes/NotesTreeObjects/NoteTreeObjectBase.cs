//-----------------------------------------------------------------------
// <copyright file="NoteTreeObjectBase.cs" company="Ace Poker Solutions">
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
using System.Xml.Serialization;

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects
{
    public abstract class NoteTreeObjectBase : ReactiveObject
    {
        #region Properties

        private bool isSelected;

        [XmlIgnore]
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isSelected, value);
            }
        }

        #endregion
    }
}