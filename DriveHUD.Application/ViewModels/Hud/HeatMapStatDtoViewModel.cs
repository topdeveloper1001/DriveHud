//-----------------------------------------------------------------------
// <copyright file="HeatMapStatDtoViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using DriveHUD.Common.Wpf.Mvvm;
using ReactiveUI;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HeatMapStatDtoViewModel : ViewModelBase
    {
        private readonly string cardRange;
        private int occurred;
        private int totalOccured;

        public HeatMapStatDtoViewModel(string cardRange)
        {
            Check.Require(cardRange != null);

            this.cardRange = cardRange;       
        }

        public string CardRange
        {
            get
            {
                return cardRange;
            }
        }

        public decimal Value
        {
            get
            {
                return totalOccured != 0 ?
                    (decimal)occurred / totalOccured :
                    default(decimal);
            }
        }

        public int Occurred
        {
            get
            {
                return occurred;
            }
            set
            {
                if (occurred == value)
                {
                    return;
                }

                occurred = value;

                this.RaisePropertyChanged(nameof(Occurred));
                this.RaisePropertyChanged(nameof(Value));
            }
        }

        public int TotalOccurred
        {
            get
            {
                return totalOccured;
            }
            set
            {
                if (totalOccured == value)
                {
                    return;
                }

                totalOccured = value;

                this.RaisePropertyChanged(nameof(TotalOccurred));
                this.RaisePropertyChanged(nameof(Value));
            }
        }
    }
}