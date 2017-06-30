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
using Model.Data;
using ReactiveUI;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HeatMapStatDtoViewModel : ViewModelBase
    {
        private readonly string cardRange;
        private readonly StatDto statDto;

        public HeatMapStatDtoViewModel(string cardRange, StatDto statDto)
        {
            Check.Require(cardRange != null);
            Check.Require(statDto != null);

            this.cardRange = cardRange;
            this.statDto = statDto;
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
                return statDto.CouldOccurred != 0 ?
                    (decimal)statDto.Occurred / statDto.CouldOccurred :
                    default(decimal);
            }
        }

        public int Occurred
        {
            get
            {
                return statDto.Occurred;
            }
            set
            {
                if (statDto.Occurred == value)
                {
                    return;
                }

                statDto.Occurred = value;

                this.RaisePropertyChanged(nameof(Occurred));
                this.RaisePropertyChanged(nameof(Value));
            }
        }

        public int CouldOccurred
        {
            get
            {
                return statDto.CouldOccurred;
            }
            set
            {
                if (statDto.CouldOccurred == value)
                {
                    return;
                }

                statDto.CouldOccurred = value;

                this.RaisePropertyChanged(nameof(CouldOccurred));
                this.RaisePropertyChanged(nameof(Value));
            }
        }
    }
}