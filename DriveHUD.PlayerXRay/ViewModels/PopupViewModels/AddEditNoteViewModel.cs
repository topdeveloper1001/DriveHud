//-----------------------------------------------------------------------
// <copyright file="AddNoteViewModel.cs" company="Ace Poker Solutions">
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
    public class AddEditNoteViewModel : ReactiveObject, IPopupInteractionAware
    {
        public AddEditNoteViewModel()
        {
            SaveCommand = ReactiveCommand.Create();
            SaveCommand.Subscribe(x =>
            {
                OnSaveAction?.Invoke();
                FinishInteraction?.Invoke();
            });

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x => FinishInteraction?.Invoke());
        }

        private string note;

        public string Note
        {
            get
            {
                return note;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref note, value);
            }
        }

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CancelCommand { get; private set; }

        public Action FinishInteraction
        {
            get;
            set;
        }

        public Action OnSaveAction
        {
            get;
            set;
        }
    }
}