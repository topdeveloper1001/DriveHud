using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes;
using System.Linq;

namespace DriveHUD.PlayerXRay.UserControls.Notes
{
    /// <summary>
    /// Interaction logic for StakesUC.xaml
    /// </summary>
    public partial class StakesUC
    {
        private List<Stake> ExcludedStakes { get; set; }

        public StakesUC()
        {
            InitializeComponent();
        }

        public void Initialize(List<Stake> excludedStakes)
        {
            ExcludedStakes = excludedStakes;
        }

        public void RefreshList(bool noLimit, bool limit, bool potLimit, bool sizeHU, bool size34, bool size56, bool size7T, bool customSize, int minVal, int maxVal)
        {
            int no = 2;
            mainStack.Children.Clear();
            StackPanel panel = new StackPanel();

            foreach (Stake stake in StaticStorage.Stakes)
            {
                if (!noLimit && stake.TableType == TableTypeEnum.NoLimit)
                    continue;
                if (!potLimit && stake.TableType == TableTypeEnum.PotLimit)
                    continue;
                if (!limit && stake.TableType == TableTypeEnum.Limit)
                    continue;

                if (!customSize)
                {
                    if (!sizeHU && stake.TableSize == TableSizeEnum.HeadsUp)
                        continue;
                    if (!size34 && stake.TableSize == TableSizeEnum.Players34)
                        continue;
                    if (!size56 && stake.TableSize == TableSizeEnum.Players56)
                        continue;
                    if (!size7T && stake.TableSize == TableSizeEnum.Player710)
                        continue;
                }
                else
                {
                    int stakeSize = (int) stake.TableSize;
                    if (!(stakeSize >= minVal && stakeSize <= maxVal))
                        continue;
                }

                if (no == 2)
                {
                    no = -1;
                    panel = new StackPanel();
                    mainStack.Children.Add(panel);
                }

                Binding bind = new Binding("IsSelected") {Source = stake, Mode = BindingMode.TwoWay};
                CheckBox box = new CheckBox {Content = stake.Name, Tag = stake.ID};
                box.SetBinding(ToggleButton.IsCheckedProperty, bind);
                panel.Children.Add(box);

                stake.IsSelected = ExcludedStakes.FindAll(p => p.Name == stake.Name).Count() == 0;
                no++;
            }
        }

        private void UserControlInitialized(object sender, EventArgs e)
        {
           UncheckAll();
        }

        public static void UncheckAll()
        {
            foreach (Stake stake in StaticStorage.Stakes)
            {
                stake.IsSelected = false;
            }
        }

        public static void CheckAll()
        {
            foreach (Stake stake in StaticStorage.Stakes)
            {
                stake.IsSelected = true;
            }
        }
    }
}
