using DriveHUD.Common.Log;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.EquityCalculator.Models;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Hand;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DriveHUD.Common.Ifrastructure;
using DriveHUD.Common.Resources;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class ExportViewModel : BaseViewModel, IInteractionRequestAware
    {
        #region Fields
        private ExportNotification _notification;
        private IEnumerable<PlayerModel> _playersList;
        private IEnumerable<CardModel> _boardCards;
        private HandHistory _currentHandHistory;
        #endregion

        #region Properties
        public Action FinishInteraction
        {
            get;
            set;
        }

        public INotification Notification
        {
            get
            {
                return this._notification;
            }

            set
            {
                if (value is ExportNotification)
                {
                    this._notification = value as ExportNotification;
                    this.PlayersList = this._notification.PlayersList;
                    this.BoardCards = this._notification.BoardCards;
                    this.CurrentHandHistory = this._notification.CurrentHandHistory;
                }
            }
        }

        public IEnumerable<PlayerModel> PlayersList
        {
            get
            {
                return _playersList;
            }

            set
            {
                _playersList = value;
            }
        }

        public IEnumerable<CardModel> BoardCards
        {
            get
            {
                return _boardCards;
            }

            set
            {
                _boardCards = value;
            }
        }

        public HandHistory CurrentHandHistory
        {
            get
            {
                return _currentHandHistory;
            }

            set
            {
                _currentHandHistory = value;
                OnPropertyChanged(nameof(IsHHExportEnabled));
            }
        }

        public bool IsHHExportEnabled
        {
            get
            {
                return _currentHandHistory != null;
            }
        }

        #endregion

        #region ICommand
        public ICommand ExportECCommand { get; set; }
        public ICommand ExportBothCommand { get; set; }
        public ICommand ExportHHCommand { get; set; }
        public ICommand CloseCommand { get; set; }



        #endregion

        public ExportViewModel()
        {
            Init();
        }

        private void Init()
        {
            ExportECCommand = new RelayCommand(ExportEC);
            ExportBothCommand = new RelayCommand(ExportBoth);
            ExportHHCommand = new RelayCommand(ExportHH);
            CloseCommand = new RelayCommand(Close);
        }

        #region HandFormatter

        private string GetBoardStringFormatted()
        {
            string boardCards = string.Empty;
            if (BoardCards != null)
            {
                boardCards = string.Join(" ", BoardCards.Select(x => x.ToString()));
            }
            return boardCards;
        }

        private IEnumerable<String> GetEquityStringFormatted(IEnumerable<PlayerModel> playersList)
        {
            List<String> returnList = new List<string>();
            foreach (var player in playersList)
            {
                String rangeString = GetRangeStringGroupHandsWithSamePrct(player);
                returnList.Add(String.Format("{0,-10}        {1,-10}        {2,-10}        {3,-10}", Math.Round(player.EquityValue, 4).ToString() + "%", Math.Round(player.WinPrct, 4).ToString() + "%", Math.Round(player.TiePrct, 4).ToString() + "%", "[ " + rangeString + " ]"));
            }
            return returnList;
        }

        private String GetRangeStringGroupHandsWithSamePrct(PlayerModel player)
        {
            String res = "";

            try
            {
                String rangeString = string.Join(",", player.GetPlayersHand(true));

                Hashtable handsGroupedByPrct = new Hashtable();
                List<int> orderedKeys = new List<int>();
                foreach (String hand in rangeString.Split(','))
                {
                    if (hand.Contains("("))
                    {
                        int key = int.Parse(hand.Substring(hand.IndexOf("(") + 1).Replace(")", "").Trim());
                        String handWithoutPrct = hand.Substring(0, hand.IndexOf("("));

                        if (handsGroupedByPrct.ContainsKey(key))
                            (handsGroupedByPrct[key] as List<String>).Add(handWithoutPrct);
                        else
                        {
                            orderedKeys.Add(key);
                            handsGroupedByPrct.Add(key, new List<String>(new String[] { handWithoutPrct }));
                        }
                    }
                    else
                    {
                        orderedKeys.Add(-1);
                        handsGroupedByPrct.Add(-1, new List<String>(new String[] { hand }));
                    }
                }

                foreach (int key in orderedKeys)
                {
                    String formattedRange = key == -1 ? (handsGroupedByPrct[key] as List<String>)[0] : GetHandsFormatted(handsGroupedByPrct[key] as List<String>, key);
                    res += formattedRange + ",";
                }
                if (res.EndsWith(",")) res = res.Substring(0, res.Length - 1);
            }
            catch (Exception exc)
            {
                LogProvider.Log.Error("ExportFunctions", "Exception occured during Equity Export", exc);
            }
            return res;
        }

        private String GetHandsFormatted(List<String> hands, int prct)
        {
            List<char> cards = new List<char>(new char[] { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' });

            int c = 0;
            List<List<String>> allGroups = new List<List<String>>();


            List<String> handsToRemove = new List<String>();
            for (int i = 0; i < hands.Count; i++)
            {
                if (hands[i].Equals("")) continue;
                if (handsToRemove.Contains(hands[i])) continue;
                List<String> group = new List<String>();
                String firstHand = hands[i];
                group.Add(firstHand);

                String lastHand = firstHand;

                bool newCardAdded = true;
                while (newCardAdded)
                {
                    int countBefore = group.Count;
                    foreach (String hand in hands)
                    {
                        if (lastHand[0] == lastHand[1] && hand[0] == hand[1] && hand != firstHand && hand != lastHand)
                        {
                            if (cards.IndexOf(lastHand[0]) == cards.IndexOf(hand[0]) - 1)
                            {
                                group.Add(hand);
                            }
                            else if (cards.IndexOf(firstHand[0]) == cards.IndexOf(hand[0]) + 1)
                            {
                                group.Insert(0, hand);
                            }
                        }
                        else if (hand != firstHand && hand != lastHand && hand.Length > 2 && firstHand.Length > 2 && hand[2] == firstHand[2])
                        {
                            if (hand[0].Equals(lastHand[0]) && cards.IndexOf(lastHand[1]) == cards.IndexOf(hand[1]) - 1)
                            {
                                group.Add(hand);
                            }
                            else if (hand[0].Equals(firstHand[0]) && cards.IndexOf(firstHand[1]) == cards.IndexOf(hand[1]) + 1)
                            {
                                group.Insert(0, hand);
                            }
                        }
                        lastHand = group[group.Count - 1];
                        firstHand = group[0];
                    }
                    newCardAdded = countBefore != group.Count;
                }

                allGroups.Add(group);
                foreach (String hand in group)
                {
                    handsToRemove.Add(hand);
                }
            }

            String prctString = prct == -1 ? "" : "(" + prct.ToString() + ")";
            String res = "";
            foreach (List<String> group in allGroups)
            {
                if (group.Count > 1)
                {
                    if ((group[0][0] != group[0][1] && cards.IndexOf(group[group.Count - 1][1]) == cards.IndexOf(group[0][0]) - 1)
                        || (group[0][0] == group[0][1] && group[group.Count - 1][0] == 'A'))
                        res += group[0] + "+" + prctString + ", ";
                    else res += group[0] + "-" + group[group.Count - 1] + prctString + ", ";
                }
                else res += group[0] + prctString + ", ";
            }
            if (res.EndsWith(", ")) res = res.Substring(0, res.Length - 2);
            return res;
        }

        #endregion

        #region ICommand implementation
        private void ExportHH(object obj)
        {
            String hh = ExportFunctions.ConvertHHToForumFormat(CurrentHandHistory);
            Clipboard.SetText(hh);
            MessageBox.Show(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString));
            this.FinishInteraction();
        }

        private void ExportBoth(object obj)
        {
            String hh = ExportFunctions.ConvertHHToForumFormat(CurrentHandHistory);

            String EquityData = ExportFunctions.GetEquityDataToExport(GetBoardStringFormatted(), GetEquityStringFormatted(this.PlayersList));
            String result = hh + Environment.NewLine + Environment.NewLine + EquityData;
            Clipboard.SetText(result);
            MessageBox.Show(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString));
            this.FinishInteraction();
        }

        private void ExportEC(object obj)
        {
            String EquityData = ExportFunctions.GetEquityDataToExport(GetBoardStringFormatted(), GetEquityStringFormatted(this.PlayersList));
            Clipboard.SetText(EquityData);
            MessageBox.Show(CommonResourceManager.Instance.GetResourceString(ResourceStrings.DataExportedMessageResourceString));
            this.FinishInteraction();
        }

        private void Close(object obj)
        {
            this.FinishInteraction();
        }


        #endregion
    }
}
