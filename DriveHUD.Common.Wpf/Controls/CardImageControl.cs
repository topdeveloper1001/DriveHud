using Model.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DriveHUD.Common.Wpf.Controls
{
    public class CardImageControl : Image, ICommandSource
    {
        public static readonly DependencyProperty SuitValueProperty =
        DependencyProperty.Register("SuitValue", typeof(RangeCardSuit), typeof(CardImageControl),
        new FrameworkPropertyMetadata(RangeCardSuit.None));

        private static readonly DependencyProperty RankValueProperty =
       DependencyProperty.Register("RankValue", typeof(RangeCardRank), typeof(CardImageControl),
       new FrameworkPropertyMetadata(RangeCardRank.None));

        public static readonly DependencyProperty IsSelectedProperty =
           DependencyProperty.Register("IsSelected", typeof(bool), typeof(CardImageControl), 
               new PropertyMetadata(false));

        public static readonly DependencyProperty CommandProperty =
           DependencyProperty.Register("Command", typeof(ICommand), typeof(CardImageControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CardImageControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty CommandTargetProperty =
          DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(CardImageControl), new UIPropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

 
        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            var command = Command;
            var parameter = CommandParameter;
            var target = CommandTarget;

            var routedCmd = command as RoutedCommand;
            if (routedCmd != null && routedCmd.CanExecute(parameter, target))
            {
                routedCmd.Execute(parameter, target);
            }
            else if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public double SuitValue
        {
            get { return (double)GetValue(SuitValueProperty); }
            set { SetValue(SuitValueProperty, value); }
        }

        public double RankValue
        {
            get { return (double)GetValue(RankValueProperty); }
            set { SetValue(RankValueProperty, value); }
        }

    }
}
