//-----------------------------------------------------------------------
// <copyright file="WindowViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Validation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace DriveHUD.Common.Wpf.Mvvm
{
    public abstract class WindowViewModel<T> : LightWindowViewModel, IWindowViewModel
        where T : WindowViewModel<T>
    {
        #region Fields

        private static RuleCollection<T> rules = new RuleCollection<T>();
        private Dictionary<string, List<object>> errors;
        private HashSet<string> validatingProperies;

        #endregion

        public WindowViewModel() : base()
        {
            Changed.Subscribe(x =>
            {
                if (x.PropertyName.Equals(nameof(HasErrors)) ||
                    x.PropertyName.Equals(nameof(IsValidating)))
                {
                    return;
                }

                if (string.IsNullOrEmpty(x.PropertyName))
                {
                    ApplyRules();
                }
                else
                {
                    ApplyRules(x.PropertyName);
                }

                this.RaisePropertyChanged(nameof(HasErrors));
            });
        }

        #region Public Events

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire object.
        /// </summary>
        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add { errorsChanged += value; }
            remove { errorsChanged -= value; }
        }

        public event EventHandler<DataErrorsChangedEventArgs> PropertyValidating;

        public event EventHandler<DataErrorsChangedEventArgs> PropertyValidated;

        public bool IsValidating
        {
            get
            {
                return validatingProperies != null &&
                    validatingProperies.Count != 0;
            }
        }

        #endregion

        #region Private Events

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire object.
        /// </summary>
        private event EventHandler<DataErrorsChangedEventArgs> errorsChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the when errors changed observable event. Occurs when the validation errors have changed for a property or for the entire object.
        /// </summary>
        /// <value>
        /// The when errors changed observable event.
        /// </value>
        public IObservable<string> WhenErrorsChanged
        {
            get
            {
                return Observable
                    .FromEventPattern<DataErrorsChangedEventArgs>(
                        h => errorsChanged += h,
                        h => errorsChanged -= h)
                    .Select(x => x.EventArgs.PropertyName);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object has validation errors.
        /// </summary>
        /// <value><c>true</c> if this instance has errors, otherwise <c>false</c>.</value>
        public virtual bool HasErrors
        {
            get
            {
                InitializeErrors();
                return errors.Count > 0;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the rules which provide the errors.
        /// </summary>
        /// <value>The rules this instance must satisfy.</value>
        protected static RuleCollection<T> Rules
        {
            get { return rules; }
        }

        /// <summary>
        /// Gets the validation errors for the entire object.
        /// </summary>
        /// <returns>A collection of errors.</returns>
        public IEnumerable GetErrors()
        {
            return GetErrors(null);
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire object.
        /// </summary>
        /// <param name="propertyName">Name of the property to retrieve errors for. <c>null</c> to
        /// retrieve all errors for this instance.</param>
        /// <returns>A collection of errors.</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            Debug.Assert(
                string.IsNullOrEmpty(propertyName) ||
                (GetType().GetRuntimeProperty(propertyName) != null),
                "Check that the property name exists for this instance.");

            InitializeErrors();

            IEnumerable result;

            if (string.IsNullOrEmpty(propertyName))
            {
                var allErrors = new List<object>();

                foreach (KeyValuePair<string, List<object>> keyValuePair in errors)
                {
                    allErrors.AddRange(keyValuePair.Value);
                }

                result = allErrors;
            }
            else
            {
                if (errors.ContainsKey(propertyName))
                {
                    result = errors[propertyName];
                }
                else
                {
                    result = new List<object>();
                }
            }

            return result;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Called when the errors have changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnErrorsChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Assert(
                string.IsNullOrEmpty(propertyName) ||
                (this.GetType().GetRuntimeProperty(propertyName) != null),
                "Check that the property name exists for this instance.");

            errorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyValidating([CallerMemberName] string propertyName = null)
        {
            Debug.Assert(
                string.IsNullOrEmpty(propertyName) ||
                (this.GetType().GetRuntimeProperty(propertyName) != null),
                "Check that the property name exists for this instance.");

            if (!validatingProperies.Contains(propertyName))
            {
                validatingProperies.Add(propertyName);
            }

            this.RaisePropertyChanged(nameof(IsValidating));

            PropertyValidating?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyValidated([CallerMemberName] string propertyName = null)
        {
            Debug.Assert(
                string.IsNullOrEmpty(propertyName) ||
                (this.GetType().GetRuntimeProperty(propertyName) != null),
                "Check that the property name exists for this instance.");

            if (validatingProperies.Contains(propertyName))
            {
                validatingProperies.Remove(propertyName);
            }

            this.RaisePropertyChanged(nameof(IsValidating));

            PropertyValidated?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ApplyRules();
        }

        #endregion

        #region Private Methods    

        /// <summary>
        /// Applies all rules to this instance.
        /// </summary>
        private void ApplyRules()
        {
            InitializeErrors();

            foreach (string propertyName in rules.Select(x => x.PropertyName))
            {
                ApplyRules(propertyName);
            }
        }

        /// <summary>
        /// Applies the rules to this instance for the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private async void ApplyRules(string propertyName)
        {
            InitializeErrors();

            var propertyErrors = new List<object>();

            if (errors.ContainsKey(propertyName))
            {
                errors[propertyName].Clear();
                OnErrorsChanged(propertyName);
            }

            OnPropertyValidating(propertyName);

            foreach (var rule in rules)
            {
                if (string.IsNullOrEmpty(propertyName) || rule.PropertyName.Equals(propertyName))
                {
                    var isValid = rule.IsAsync ? await rule.ApplyAsync((T)this) :
                        rule.Apply((T)this);

                    if (!isValid)
                    {
                        propertyErrors.Add(rule.Error);
                    }
                }
            }

            if (propertyErrors.Count > 0)
            {
                if (errors.ContainsKey(propertyName))
                {
                    errors[propertyName].Clear();
                }
                else
                {
                    errors[propertyName] = new List<object>();
                }

                errors[propertyName].AddRange(propertyErrors);
                OnErrorsChanged(propertyName);
            }
            else if (errors.ContainsKey(propertyName))
            {
                errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }

            OnPropertyValidated(propertyName);
        }

        /// <summary>
        /// Initializes the errors and applies the rules if not initialized.
        /// </summary>
        private void InitializeErrors()
        {
            if (errors == null)
            {
                errors = new Dictionary<string, List<object>>();
            }

            if (validatingProperies == null)
            {
                validatingProperies = new HashSet<string>();
            }
        }

        #endregion
    }
}