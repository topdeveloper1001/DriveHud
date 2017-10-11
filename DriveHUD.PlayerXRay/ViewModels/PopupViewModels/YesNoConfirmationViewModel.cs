//-----------------------------------------------------------------------
// <copyright file="YesNoConfirmationViewModel.cs" company="Ace Poker Solutions">
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
using System;

namespace DriveHUD.PlayerXRay.ViewModels.PopupViewModels
{
    public class YesNoConfirmationViewModel : ReactiveObject, IPopupInteractionAware
    {
        public YesNoConfirmationViewModel()
        {
            YesCommand = ReactiveCommand.Create();
            YesCommand.Subscribe(x =>
            {
                OnYesAction?.Invoke();
                FinishInteraction?.Invoke();
            });

            NoCommand = ReactiveCommand.Create();
            NoCommand.Subscribe(x =>
            {
                OnNoAction?.Invoke();
                FinishInteraction?.Invoke();
            });
        }

        private string сonfirmationMessage;

        public string ConfirmationMessage
        {
            get
            {
                return сonfirmationMessage;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref сonfirmationMessage, value);
            }
        }

        public ReactiveCommand<object> YesCommand { get; private set; }

        public ReactiveCommand<object> NoCommand { get; private set; }

        public Action FinishInteraction
        {
            get;
            set;
        }

        public Action OnYesAction
        {
            get;
            set;
        }

        public Action OnNoAction
        {
            get;
            set;
        }
    }
}
