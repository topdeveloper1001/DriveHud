using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AcePokerSolutions.UIControls.CustomControls
{
    /// <summary>
    /// Interaction logic for HoleCardsUC.xaml
    /// </summary>
    public partial class HoleCardsUC
    {
        private readonly SolidColorBrush m_selectedBrush = new SolidColorBrush(Color.FromArgb(30, 255, 255, 255));
        private readonly SolidColorBrush m_transparentBrush = new SolidColorBrush(Color.FromRgb(53, 53, 53));
        public List<string> ExcludedCards = new List<string>();

        public HoleCardsUC()
        {
            InitializeComponent();
        }

        public void Initialize(List<string> excludedList)
        {
            SelectAll();
            ExcludedCards = new List<string>();

            foreach (UIElement elem in grid.Children)
            {
                if (!(elem is Label))
                    continue;
                Label label = (Label) elem;

                if (!excludedList.Contains(label.Content.ToString())) continue;
                label.Background = m_transparentBrush;
                ExcludedCards.Add(label.Content.ToString());
            }
        }

        private void SelectAll()
        {
            ExcludedCards.Clear();
            foreach (UIElement elem in grid.Children)
            {
                if (!(elem is Label))
                    continue;

                Label label = (Label)elem;
                label.Background = m_selectedBrush;
            }
        }

        private void DeselectAll()
        {
            ExcludedCards.Clear();

            foreach (UIElement elem in grid.Children)
            {
                if (!(elem is Label))
                    continue;

                Label label = (Label)elem;
                label.Background = m_transparentBrush;
                ExcludedCards.Add(label.Content.ToString());
            }
        }

        private void LabelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Label tb = (Label)sender;

            if (ExcludedCards.Contains(tb.Content.ToString()))
            {
                ExcludedCards.Remove(tb.Content.ToString());
                tb.Background = m_selectedBrush;
                
            }
            else
            {
                ExcludedCards.Add(tb.Content.ToString());
                tb.Background = m_transparentBrush;
            }
        }

        private void LabelMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
           if (e.LeftButton == MouseButtonState.Released)
                return;

            LabelMouseLeftButtonDown(sender, null);
        }

        private int GetCardValue(char card)
        {
            int val;
            Int32.TryParse(card.ToString(), out val);

            if (val > 0)
                return val;

            switch (card)
            {
                case 'T':
                    return 10;
                case 'J':
                    return 11;
                case 'Q':
                    return 12;
                case 'K':
                    return 13;
                case 'A':
                    return 14;
            }

            return 0;
        }

        private void btnSuitedConnectors_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (UIElement elem in grid.Children)
            {
                if (!(elem is Label))
                    continue;

                Label label = (Label)elem;
                string text = label.Content.ToString();
                if (!(text.Contains("s")))
                    continue;

                int val1 = GetCardValue(text[0]);
                int val2 = GetCardValue(text[1]);

                if (val1 - val2 == 1)
                    LabelMouseLeftButtonDown(label, null);
            }
        }

        private void btnUnsuitedConnectors_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (UIElement elem in grid.Children)
            {
                if (!(elem is Label))
                    continue;

                Label label = (Label)elem;
                string text = label.Content.ToString();
                if (!(text.Contains("o")))
                    continue;

                int val1 = GetCardValue(text[0]);
                int val2 = GetCardValue(text[1]);

                if (val1 - val2 == 1)
                    LabelMouseLeftButtonDown(label, null);
            }
        }

        private void btnSuitedCards_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (UIElement elem in grid.Children)
            {
                if (!(elem is Label))
                    continue;

                Label label = (Label)elem;
                if (label.Content.ToString().Contains("s"))
                    LabelMouseLeftButtonDown(label, null);
            }
        }

        private void btnUnsuitedCards_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (UIElement elem in grid.Children)
            {
                if (!(elem is Label))
                    continue;

                Label label = (Label)elem;
                if (label.Content.ToString().Contains("o"))
                    LabelMouseLeftButtonDown(label, null);
            }
        }

        private void btnPocketPairs_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (UIElement elem in grid.Children)
            {
                if (!(elem is Label))
                    continue;

                Label label = (Label)elem;
                string content = label.Content.ToString();

                if (content[0] == content[1])
                    LabelMouseLeftButtonDown(label, null);
            }
        }

        private void btnSelectAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           SelectAll();
        }

        private void btnDeselectAll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	DeselectAll();
        }
    }
}
