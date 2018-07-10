//-----------------------------------------------------------------------
// <copyright file="Validation.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class Validation
    {
        public static readonly DependencyProperty AsyncValidationProperty = DependencyProperty.RegisterAttached("AsyncValidation",
            typeof(bool), typeof(Validation), new PropertyMetadata(OnAsyncValidationPropertyChanged));

        public static void SetAsyncValidation(FrameworkElement element, bool value)
        {
            element.SetValue(AsyncValidationProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static bool GetAsyncValidation(FrameworkElement element)
        {
            return (bool)element.GetValue(AsyncValidationProperty);
        }

        public static readonly DependencyProperty ValidationTemplateProperty = DependencyProperty.RegisterAttached("ValidationTemplate",
            typeof(ControlTemplate), typeof(Validation), new PropertyMetadata(null));

        public static void SetValidationTemplate(FrameworkElement element, ControlTemplate value)
        {
            element.SetValue(ValidationTemplateProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(FrameworkElement))]
        public static ControlTemplate GetValidationTemplate(FrameworkElement element)
        {
            return (ControlTemplate)element.GetValue(ValidationTemplateProperty);
        }

        private static readonly DependencyProperty ValidationLoadingAdornerProperty = DependencyProperty.RegisterAttached("ValidationLoadingAdorner",
                       typeof(ValidationLoadingAdorner), typeof(Validation),
                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.NotDataBindable));

        private static void OnAsyncValidationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is FrameworkElement element))
            {
                return;
            }

            void handler(object s, DependencyPropertyChangedEventArgs arg)
            {
                element.DataContextChanged -= handler;

                if (!(arg.NewValue is IValidationAsync validationContext))
                {
                    return;
                }

                validationContext.PropertyValidating += (o, a) =>
                {
                    ShowPropertyValidatingContent(element, a.PropertyName, true);
                };

                validationContext.PropertyValidated += (o, a) =>
                {
                    ShowPropertyValidatingContent(element, a.PropertyName, false);
                };
            }

            element.DataContextChanged += handler;
        }

        private static void ShowPropertyValidatingContent(FrameworkElement element, string propertyName, bool show)
        {
            if (!CheckTextPropertyBinding(element, propertyName))
            {
                return;
            }

            ShowValidationAdornerHelper(element, show);
        }

        private static void ShowValidationAdornerHelper(FrameworkElement element, bool show)
        {
            element.IsEnabled = !show;

            var adornerLayer = AdornerLayer.GetAdornerLayer(element);

            if (adornerLayer == null)
            {
                return;
            }

            var validationAdorner = element.ReadLocalValue(ValidationLoadingAdornerProperty) as ValidationLoadingAdorner;

            if (show && validationAdorner == null)
            {
                var validationTemplate = GetValidationTemplate(element);

                if (validationTemplate != null)
                {
                    validationAdorner = new ValidationLoadingAdorner(element, validationTemplate);
                    adornerLayer.Add(validationAdorner);
                    element.SetValue(ValidationLoadingAdornerProperty, validationAdorner);
                }
            }
            else if (!show && validationAdorner != null)
            {
                validationAdorner.ClearChild();
                adornerLayer.Remove(validationAdorner);

                element.ClearValue(ValidationLoadingAdornerProperty);
            }
        }

        private static bool CheckTextPropertyBinding(FrameworkElement element, string propertyName)
        {
            var binding = BindingOperations.GetBinding(element, TextBox.TextProperty);
            return binding != null && binding.Path.Path.Equals(propertyName);
        }
    }
}