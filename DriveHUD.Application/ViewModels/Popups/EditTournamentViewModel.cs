using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using Microsoft.Practices.ServiceLocation;
using Model.Events;
using Model.Interfaces;
using Model.Reports;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class EditTournamentViewModel : BaseViewModel
    {
        public EditTournamentViewModel()
        {
            _dataService = ServiceLocator.Current.GetInstance<IDataService>();

            Initialize();
        }

        private void Initialize()
        {
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

            TournamentSpeedItems = new List<TournamentSpeed>()
            {
                TournamentSpeed.Regular, TournamentSpeed.Turbo, TournamentSpeed.SuperTurbo, TournamentSpeed.HyperTurbo
            };

            var gametypes = Enum.GetValues(typeof(GameType)).Cast<GameType>().Where(x => x != GameType.Any && x != GameType.Unknown).ToList();
            GameTypeItems = new List<GameType>(gametypes);

            TournamentTypeItems = new List<TournamentsTags>()
            {
                TournamentsTags.MTT, TournamentsTags.STT
            };
        }

        public void LoadTournament(string playerName, string tournamentId, short pokerId)
        {
            TournamentEntity = _dataService.GetTournament(tournamentId, playerName, pokerId);
            BuyIn = TournamentEntity.Buyinincents / 100m;
            Place = TournamentEntity.Finishposition;
            WonAmount = TournamentEntity.Winningsincents / 100m;
            Rebuy = TournamentEntity.Rebuyamountincents / 100m;
            Rake = TournamentEntity.Rakeincents / 100m;
            TableSize = TournamentEntity.Tablesize;

            TournamentSpeedSelectedItem = (TournamentSpeed)TournamentEntity.SpeedtypeId;
            GameTypeSelectedItem = (GameType)TournamentEntity.PokergametypeId;

            if (Enum.TryParse(TournamentEntity.Tourneytagscsv, out TournamentsTags tourneyType))
            {
                TournamentTypeSelectedItem = tourneyType;
            }
        }

        private void UpdateEntity()
        {
            if (TournamentEntity == null)
            {
                return;
            }

            TournamentEntity.Buyinincents = (int)(BuyIn * 100m);
            TournamentEntity.Finishposition = Place;
            TournamentEntity.Winningsincents = (int)(WonAmount * 100m);
            TournamentEntity.Rebuyamountincents = (int)(Rebuy * 100m);
            TournamentEntity.Rakeincents = (int)(Rake * 100m);
            TournamentEntity.SpeedtypeId = (short)TournamentSpeedSelectedItem;
            TournamentEntity.PokergametypeId = (short)GameTypeSelectedItem;
            TournamentEntity.Tourneytagscsv = TournamentTypeSelectedItem.ToString();
            TournamentEntity.Tablesize = TableSize;

            _dataService.Store(TournamentEntity);
        }

        private void Save(object obj)
        {
            UpdateEntity();

            var reportStatusService = ServiceLocator.Current.GetInstance<IReportStatusService>();
            reportStatusService.TournamentUpdated = true;

            ServiceLocator.Current
                .GetInstance<IEventAggregator>()
                .GetEvent<TournamentDataUpdatedEvent>()
                .Publish(new TournamentDataUpdatedEventArgs());

            if (CloseAction != null)
            {
                CloseAction.Invoke();
            }
        }

        private void Cancel(object obj)
        {
            if (CloseAction != null)
            {
                CloseAction.Invoke();
            }
        }

        #region ICommand

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        #endregion

        #region Properties

        private IDataService _dataService;

        private IEnumerable<TournamentSpeed> _tournamentSpeedItems;
        private IEnumerable<GameType> _gameTypeItems;
        private IEnumerable<TournamentsTags> _tournamentTypeItems;

        private Tournaments _tournamentEntity;
        private decimal _buyIn;
        private int _place;
        private decimal _wonAmout;
        private decimal _rebuy;
        private decimal _rake;
        private short _tableSize;
        private TournamentSpeed _tournamentSpeedSelectedItem;
        private GameType _gameTypeSelecetdItem;
        private TournamentsTags _tournamentTypeSelectedItem;

        public GameType GameTypeSelectedItem
        {
            get { return _gameTypeSelecetdItem; }
            set { _gameTypeSelecetdItem = value; }
        }

        public TournamentSpeed TournamentSpeedSelectedItem
        {
            get { return _tournamentSpeedSelectedItem; }
            set { SetProperty(ref _tournamentSpeedSelectedItem, value); }
        }

        public TournamentsTags TournamentTypeSelectedItem
        {
            get { return _tournamentTypeSelectedItem; }
            set { SetProperty(ref _tournamentTypeSelectedItem, value); }
        }

        public IEnumerable<TournamentSpeed> TournamentSpeedItems
        {
            get { return _tournamentSpeedItems; }
            set { SetProperty(ref _tournamentSpeedItems, value); }
        }

        public IEnumerable<GameType> GameTypeItems
        {
            get { return _gameTypeItems; }
            set { SetProperty(ref _gameTypeItems, value); }
        }

        public IEnumerable<TournamentsTags> TournamentTypeItems
        {
            get { return _tournamentTypeItems; }
            set { SetProperty(ref _tournamentTypeItems, value); }
        }

        public decimal Rake
        {
            get { return _rake; }
            set
            {
                if (value < 0) value = 0;
                SetProperty(ref _rake, value);
            }
        }

        public decimal Rebuy
        {
            get { return _rebuy; }
            set
            {
                if (value < 0) value = 0;
                SetProperty(ref _rebuy, value);
            }
        }

        public decimal WonAmount
        {
            get { return _wonAmout; }
            set
            {
                if (value < 0) value = 0;
                SetProperty(ref _wonAmout, value);
            }
        }

        public int Place
        {
            get { return _place; }
            set
            {
                if (value < 1) value = 1;
                SetProperty(ref _place, value);
            }
        }

        public decimal BuyIn
        {
            get { return _buyIn; }
            set
            {
                if (value < 0) value = 0;
                SetProperty(ref _buyIn, value);
            }
        }

        public Tournaments TournamentEntity
        {
            get { return _tournamentEntity; }
            set { _tournamentEntity = value; }
        }

        public short TableSize
        {
            get
            {
                return _tableSize;
            }
            set
            {
                if (value < 2)
                {
                    value = 2;
                }
                else if (value > 11)
                {
                    value = 10;
                }

                SetProperty(ref _tableSize, value);
            }
        }

        public Action CloseAction;

        #endregion
    }
}