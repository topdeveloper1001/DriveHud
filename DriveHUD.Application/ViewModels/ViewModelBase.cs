using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Linq.Expressions;
using DriveHUD.Common.Reflection;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Base view model class inherited from ReactiveUI (do not delete, probably this one will replace BaseViewModel)
    /// </summary>
    public abstract class ViewModelBase : ReactiveObject, IDisposable
    {
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            this.RaisePropertyChanged(ReflectionHelper.GetPropertyNameFromExpression(property));
        }

        #region Properties

        private bool isActive;

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isActive, value);
            }
        }

        #endregion

        #region IDisposable implementation

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
                    DisposeList.Dispose();
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

        private CompositeDisposable disposeList = new CompositeDisposable();

        protected CompositeDisposable DisposeList
        {
            get { return disposeList; }
        }

        #endregion
    }
}