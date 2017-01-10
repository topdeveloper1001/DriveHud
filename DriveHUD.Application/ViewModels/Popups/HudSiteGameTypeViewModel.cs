using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Site;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ViewModels.Popups
{
    public class HudSiteGameTypeViewModel : BaseViewModel
    {
        private EnumGameTypeWrapper _gameType;
        private List<EnumGameTypeWrapper> _gameTypes;
        private EnumPokerSitesWrapper _selectedPokerSite;
        private bool? _result;

        public ICommand OkCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        public bool? Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        public EnumPokerSitesWrapper SelectedPokerSite
        {
            get { return _selectedPokerSite; }
            set { SetProperty(ref _selectedPokerSite, value); }
        }

        public EnumGameTypeWrapper SelectedGameType
        {
            get { return _gameType; }
            set { SetProperty(ref _gameType, value); }
        }

        public List<EnumGameTypeWrapper> GameTypes
        {
            get { return _gameTypes; }
            set { SetProperty(ref _gameTypes, value); }
        }

        public ObservableCollection<EnumPokerSitesWrapper> PokerSites { get; private set; }
        public Action CloseAction;
        
        public HudSiteGameTypeViewModel(EnumPokerSites pokerSite, EnumGameType gameType)
        {
            Result = null;
            OkCommand = new DelegateCommand(OkExecuted);
            CancelCommand = new DelegateCommand(CancelExecuted);
            GameTypes =
                Enum.GetValues(typeof(EnumGameType))
                    .Cast<EnumGameType>()
                    .Select(x => new EnumGameTypeWrapper(x))
                    .ToList();
            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            var configurations = configurationService.GetAll();
            PokerSites =
                new ObservableCollection<EnumPokerSitesWrapper>(
                    configurations.Select(c => new EnumPokerSitesWrapper(c.Site)));
            SelectedPokerSite = PokerSites.FirstOrDefault(p => p.PokerSite == pokerSite);
            SelectedGameType = GameTypes.FirstOrDefault(x => x.GameType == gameType);
        }

        private void CancelExecuted(object obj)
        {
            Result = false;
            CloseAction.Invoke();
        }

        private void OkExecuted(object obj)
        {
            Result = true;
            CloseAction.Invoke();
        }
    }
}