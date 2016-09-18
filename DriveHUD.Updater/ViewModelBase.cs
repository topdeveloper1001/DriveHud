//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace DriveHUD.Updater
{
    public class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            OnPropertyChanged(ReflectionHelper.GetPropertyNameFromExpression(property));
        }      

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {                 
                    // Dispose managed resources here.                 
                    Disposing();
                }

                // Clear unmanaged resources here

                disposed = true;
            }
        }

        protected virtual void Disposing()
        {
        }

        ~ViewModelBase()
        {
            Dispose(false);
        }    
       
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {           
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion              
    }
}