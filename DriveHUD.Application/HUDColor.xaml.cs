using DriveHUD.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace DriveHUD.Application
{
    /// <summary>
    /// Interaction logic for Color.xaml
    /// </summary>
    public partial class HUDColor : RadWindow
    {
        Hashtable colorList = new Hashtable();
        public HUDColor()
        {
            InitializeComponent();
        }

        public HUDColor(string selectedValue, HudViewModel ViewModel)
        {            
            InitializeComponent();
            foreach (StatInfo st in ViewModel.HudStats)
            {
                if (st.PropertyName == selectedValue)
                {
                    this.DataContext = st;

                    if (st.ColorList != null)
                    {
                        colorList = st.ColorList;
                    }                    
                    break;
                }                
            }           
            ParseData();            
        }

        private void ParseData()
        {
            try
            {
                if (colorList != null)
                {
                    foreach (object ky in colorList.Keys)
                    {
                        string[] strName = ky.ToString().Split('_');
                        if (strName.Count() > 0)
                        {
                            switch (strName[0].ToUpper())
                            {
                                case "LBLACTIVE1":
                                    SetColor(strName[0], colorList[ky]);
                                    chkActive1.IsChecked = true;
                                    break;
                                case "LBLACTIVE2":
                                    SetColor(strName[0], colorList[ky]);
                                    chkActive2.IsChecked = true;
                                    break;
                                case "LBLACTIVE3":
                                    SetColor(strName[0], colorList[ky]);
                                    chkActive3.IsChecked = true;
                                    break;
                                case "LBLACTIVE4":
                                    SetColor(strName[0], colorList[ky]);
                                    chkActive4.IsChecked = true;
                                    break;
                                case "LBLACTIVE5":
                                    SetColor(strName[0], colorList[ky]);
                                    chkActive5.IsChecked = true;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        private void SetColor(string value,object cl)
        {
            try
            {
                System.Windows.Controls.TextBlock lbl = (System.Windows.Controls.TextBlock)this.FindName(value);
                if (lbl != null)
                {
                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString(cl.ToString());

                    lbl.Background = brush;
                }
            }
            catch (Exception)
            {                
                throw;
            }
        }

        private void colorPicker_SelectedColorChanged(object sender, EventArgs e)
        {
            try
            {
                RadColorPicker colpick = sender as RadColorPicker;

                SetColor(colpick.Tag.ToString(), (object)colpick.SelectedColor);

                Brush brush = new SolidColorBrush(colpick.SelectedColor);
                colpick.Background = brush;               
            }
            catch (Exception)
            {                
                throw;
            }
        }

        private void chkActive1_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox colpick = sender as CheckBox;
                if ((bool)colpick.IsChecked)
                {
                    colpick.Content = "Active";
                }
                else
                {
                    colpick.Content = "In Active";
                }
            }
            catch (Exception)
            {                
            }
        }

        private void RadWindow_Closed_1(object sender, WindowClosedEventArgs e)
        {
            try
            {
                StatInfo st = this.DataContext as StatInfo;
                st.ColorList = new Hashtable();
                if ((bool)chkActive1.IsChecked)
                {
                    Add("lblActive1_" + txtActive1.Text, RadActive1.SelectedColor, st.ColorList);
                }
                
                if ((bool)chkActive2.IsChecked)
                {
                    Add("lblActive2_" + txtActive2.Text, RadActive2.SelectedColor, st.ColorList);
                }
                
                if ((bool)chkActive3.IsChecked)
                {
                    Add("lblActive3_" + txtActive3.Text, RadActive3.SelectedColor, st.ColorList);
                }
                
                if ((bool)chkActive4.IsChecked)
                {
                    Add("lblActive4_" + txtActive4.Text, RadActive4.SelectedColor, st.ColorList);
                }
                
                if ((bool)chkActive5.IsChecked)
                {
                    Add("lblActive5_" + txtActive5.Text, RadActive5.SelectedColor, st.ColorList);
                }                
            }   
            catch (Exception)
            {                
                throw;
            }
        }

        private void Add(string data,Color cl,Hashtable ht)
        {
            try
            {        
                if (ht.Contains(data))
                {
                    ht[data] = cl;
                }
                else
                {
                    ht.Add(data, cl);
                }
            }
            catch (Exception)
            {                
                throw;
            }
        }

        private void txtActive1_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtActive1.Text))
                {
                    lblActive1.Text = "Less than" + txtActive1.Text;                    
                    lblActive2.Text = "Between " + txtActive1.Text + " and " + txtActive2.Text;                    
                }
            }
            catch (Exception)
            {                
             
            }
        }
        private void txtActive2_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtActive1.Text))
                {
                    lblActive2.Text = "Between " + txtActive1.Text + " and " + txtActive2.Text;                    
                    lblActive3.Text = "Between " + txtActive2.Text + " and " + txtActive3.Text;
                }
            }
            catch (Exception)
            {

            }
        }
        private void txtActive3_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtActive1.Text))
                {
                    lblActive3.Text = "Between " + txtActive2.Text + " and " + txtActive3.Text;
                    lblActive4.Text = "Between " + txtActive4.Text + " and " + txtActive5.Text;
                }
            }
            catch (Exception)
            {

            }
        }
        private void txtActive4_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtActive1.Text))
                {
                    lblActive4.Text = "Between " + txtActive3.Text + " and " + txtActive4.Text;
                    lblActive5.Text = "More than " + txtActive5.Text;
                }
            }
            catch (Exception)
            {

            }
        }

        private void txtActive5_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtActive5.Text))
                {
                    lblActive5.Text = "More than " + txtActive5.Text;
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
