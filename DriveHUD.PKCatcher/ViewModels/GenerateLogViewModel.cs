//-----------------------------------------------------------------------
// <copyright file="GenerateLogViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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

namespace DriveHUD.PKCatcher.ViewModels
{
    public class GenerateLogViewModel : ReactiveObject, IPopupInteractionAware
    {
        public GenerateLogViewModel()
        {
            CloseCommand = ReactiveCommand.Create(() => FinishInteraction?.Invoke());
        }

        public ReactiveCommand CloseCommand { get; private set; }

        public string Message { get; set; }

        #region Implementation of IPopupInteractionAware

        private Action finishInteraction;

        public Action FinishInteraction
        {
            get
            {
                return finishInteraction;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref finishInteraction, value);
            }
        }

        #endregion
    }
}