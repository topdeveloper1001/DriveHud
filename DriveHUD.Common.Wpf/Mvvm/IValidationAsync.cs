using System;
using System.ComponentModel;

namespace DriveHUD.Common.Wpf.Mvvm
{
    public interface IValidationAsync
    {
        bool IsValidating { get; }

        event EventHandler<DataErrorsChangedEventArgs> PropertyValidating;

        event EventHandler<DataErrorsChangedEventArgs> PropertyValidated;
    }
}