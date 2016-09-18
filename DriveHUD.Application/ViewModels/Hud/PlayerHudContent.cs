//-----------------------------------------------------------------------
// <copyright file="PlayerHudContent.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.ViewModels;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels
{
    [XmlInclude(typeof(StatInfoBreak))]
    [Serializable]
    public class PlayerHudContent : ViewModelBase
    {
        public PlayerHudContent()
        {         
        }

        #region Properties

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }

        private int seatNumber;

        public int SeatNumber
        {
            get
            {
                return seatNumber;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref seatNumber, value);
            }
        }      

        [NonSerialized]
        private HudElementViewModel hudElement;

        [XmlIgnore]
        public HudElementViewModel HudElement
        {
            get
            {
                return hudElement;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref hudElement, value);
            }
        }
    }

    #endregion
}