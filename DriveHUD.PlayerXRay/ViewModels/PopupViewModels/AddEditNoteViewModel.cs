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
            var canSave = this.WhenAny(x => x.Name, x => !string.IsNullOrWhiteSpace(x.Value));

            SaveCommand = ReactiveCommand.Create(canSave);
            SaveCommand.Subscribe(x =>
            {
                OnSaveAction?.Invoke();
                FinishInteraction?.Invoke();
            });

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x => FinishInteraction?.Invoke());
        }

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

        private bool isGroupPossible;

        public bool IsGroupPossible
        {
            get
            {
                return isGroupPossible;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isGroupPossible, value);
            }
        }

        private bool isGroup;

        public bool IsGroup
        {
            get
            {
                return isGroup;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isGroup, value);
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