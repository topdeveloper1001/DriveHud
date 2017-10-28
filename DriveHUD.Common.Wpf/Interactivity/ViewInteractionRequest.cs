//-----------------------------------------------------------------------
// <copyright file="ViewInteractionRequest.cs" company="Ace Poker Solutions">
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
using System;

namespace DriveHUD.Common.Wpf.Interactivity
{
    public class ViewInteractionRequest : IViewInteractionRequest
    {
        public event EventHandler<ViewInteractionRequestEventArgs> Raised;

        public virtual void Raise(string viewName)
        {
            Raise(viewName, null);
        }

        public virtual void Raise(ViewModelInfo modelInfo)
        {
            Raise(null, modelInfo);
        }

        public void Raise<TViewModel>(Action<TViewModel> availableCallback) where TViewModel : class
        {
            Raise(null, null, availableCallback);
        }

        public void Raise<TViewModel>(ViewModelInfo modelInfo, Action<TViewModel> availableCallback) where TViewModel : class
        {
            Raise(null, modelInfo, availableCallback);
        }

        public void Raise(string viewName, ViewModelInfo context)
        {
            Raised?.Invoke(this, new ViewInteractionRequestEventArgs(viewName, context, null));
        }

        public void Raise<TViewModel>(string viewName, Action<TViewModel> availableCallback)
            where TViewModel : class
        {
            Raise(viewName, null, availableCallback);
        }

        public virtual void Raise<TViewModel>(string viewName, ViewModelInfo context, Action<TViewModel> onAvailable)
            where TViewModel : class
        {
            Raised?.Invoke(this, new ViewInteractionRequestEventArgs(
                    viewName, context,
                    view =>
                    {
                        if (onAvailable != null)
                        {
                            var viewModel = ViewModelContainerHelper.GetViewModelAs<TViewModel>(view);
                            onAvailable.Invoke(viewModel);
                        }
                    })
           );
        }
    }
}