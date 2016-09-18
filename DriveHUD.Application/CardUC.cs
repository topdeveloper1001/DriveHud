using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Model.Extensions;

namespace DriveHUD.Application
{
    public class CardUC: StackPanel
    {
        public string Cards
        {
            get { return (string)GetValue(CardsProperty); }
            set { SetValue(CardsProperty, value); }
        }

        public static readonly DependencyProperty CardsProperty =
            DependencyProperty.Register("Cards", typeof(string), typeof(CardUC), new PropertyMetadata(string.Empty, OCardsChanged));

        private static void OCardsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var uc = d as CardUC;
            uc.CreateCards();
        }

        public virtual void CreateCards()
        {
            Children.Clear();
            if (string.IsNullOrEmpty(Cards)) 
                return;
            
            foreach (var card in CardHelper.Split(Cards))
            {
                var border = new Border {  HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment=VerticalAlignment.Stretch,BorderThickness=new Thickness(0,1,0,1) ,CornerRadius = new CornerRadius(2)};
                switch(card.Last())
                {
                    case 's':
                        border.Background=App.Current.Resources["BlackSpades_CardBackground"] as Brush;
                        border.BorderBrush = App.Current.Resources["BlackSpades_CardBorder"] as Brush;
                        break;
                    case 'h':
                        border.Background = App.Current.Resources["RedHearts_CardBackground"] as Brush;
                        border.BorderBrush = App.Current.Resources["RedHearts_CardBorder"] as Brush;
                        break;
                    case 'd':
                        border.Background = App.Current.Resources["BlueDiamonds_CardBackground"] as Brush;
                        border.BorderBrush = App.Current.Resources["BlueDiamonds_CardBorder"] as Brush;
                        break;
                    case 'c':
                        border.Background = App.Current.Resources["GreenClubs_CardBackground"] as Brush;
                        border.BorderBrush = App.Current.Resources["GreenClubs_CardBorder"] as Brush;
                        break;
                            
                }

                border.Child = new Label { HorizontalContentAlignment=HorizontalAlignment.Center, Foreground = new SolidColorBrush(Colors.Black), FontFamily = new FontFamily("Open Sans"), FontSize = 18, Content = card.Substring(0, card.Length - 1),VerticalAlignment=VerticalAlignment.Center,Padding=new Thickness(0),HorizontalAlignment=HorizontalAlignment.Center };
                var outBorder = new Border
                {
                    Width = 22,
                    Height = 33,
                    Margin = new Thickness(1),
                    BorderThickness = new Thickness(1),
                    BorderBrush = App.Current.Resources["OutCardBorder"] as Brush,
                    CornerRadius = new CornerRadius(2),
                    Child = border
                };
                    
                Children.Add(outBorder);
                    
            }
        }
        public CardUC()
        {
            Orientation = Orientation.Horizontal;
        }
    }
}