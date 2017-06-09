using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.Controls
{
    /// <summary>
    /// Interaction logic for HudTextBoxDesigner.xaml
    /// </summary>
    public partial class HudTextBoxDesigner : UserControl
    {
        public HudTextBoxDesigner()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as DependencyObject;

            if (textBox == null)
            {
                return;
            }

            var shape = FindParent<RadDiagramShape>(textBox);

            if (shape != null)
            {
                shape.IsSelected = true;
            }

            e.Handled = true;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            // get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            // we've reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we're looking for
            var parent = parentObject as T;

            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }
    }
}