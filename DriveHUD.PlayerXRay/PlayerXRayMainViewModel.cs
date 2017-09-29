﻿//-----------------------------------------------------------------------
// <copyright file="AppStoreViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.PlayerXRay
{
    public class PlayerXRayMainViewModel : WindowViewModelBase, INotification
    {
        public PlayerXRayMainViewModel()
        {
            title = "Player X-Ray";
        }

        public void Initialize()
        {
            Console.WriteLine($"{this} was initialized");
        }

        #region INotification implementation

        private string title;

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref title, value);
            }
        }

        private object content;

        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref content, value);
            }
        }

        #endregion
    }
}