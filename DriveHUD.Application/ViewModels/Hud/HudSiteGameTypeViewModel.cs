using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common;
using DriveHUD.Common.Infrastructure.Base;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Site;
using ReactiveUI;

namespace DriveHUD.Application.ViewModels
{
    public class HudSelectSiteGameTypeViewModel : BaseViewModel
    {
        private readonly HudSelectSiteGameTypeViewModelInfo _viewModelInfo;
        private EnumGameTypeWrapper _gameType;
        private List<EnumGameTypeWrapper> _gameTypes;
        private EnumPokerSitesWrapper _selectedPokerSite;

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CancelCommand { get; private set; }

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

        public HudSelectSiteGameTypeViewModel(HudSelectSiteGameTypeViewModelInfo viewModelInfo)
        {
            Check.ArgumentNotNull(() => viewModelInfo);
            _viewModelInfo = viewModelInfo;
            Initialize();
        }

        private void Initialize()
        {
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
            SelectedPokerSite = PokerSites.FirstOrDefault(p => p.PokerSite == _viewModelInfo.PokerSite);
            SelectedGameType = GameTypes.FirstOrDefault(x => x.GameType == _viewModelInfo.GameType);
            SaveCommand = ReactiveCommand.Create();
            SaveCommand.Subscribe(x =>
            {
                _viewModelInfo.Save?.Invoke();
            });

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x =>
            {
                _viewModelInfo.Cancel?.Invoke();
            });
        }
    }
}