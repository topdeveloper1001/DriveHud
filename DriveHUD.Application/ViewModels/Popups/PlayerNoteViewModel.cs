using DriveHUD.Common.Infrastructure.Base;
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Entities;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Popups
{
    public class PlayerNoteViewModel : BaseViewModel
    {
        public PlayerNoteViewModel(short pokerSiteId, string playerName)
        {
            SaveCommand = new RelayCommand(Save);
            _dataService = ServiceLocator.Current.GetInstance<IDataService>();

            PokersiteId = pokerSiteId;
            PlayerName = playerName;
            Note = string.Empty;

            LoadNote();
        }

        private void LoadNote()
        {
            _playerNoteEntity = _dataService.GetPlayerNote(PlayerName, PokersiteId);
            if (_playerNoteEntity != null)
            {
                Note = _playerNoteEntity.Note;
            }
        }

        #region Properties
        private IDataService _dataService;

        private string _note;
        private string _playerName;
        private Playernotes _playerNoteEntity;
        private short _pokersiteId;

        public string Note
        {
            get { return _note; }
            set { SetProperty(ref _note, value); }
        }

        public string PlayerName
        {
            get { return _playerName; }
            set
            {
                SetProperty(ref _playerName, value);
            }
        }

        public Playernotes PlayerNoteEntity
        {
            get { return _playerNoteEntity; }
        }

        public short PokersiteId
        {
            get { return _pokersiteId; }
            set { SetProperty(ref _pokersiteId, value); }
        }

        public Action CloseAction { get; set; }

        #endregion

        #region ICommand

        public ICommand SaveCommand { get; set; }

        #endregion

        #region ICommand implementation

        private void Save()
        {
            if (PlayerNoteEntity == null)
            {
                var player = _dataService.GetPlayer(PlayerName, PokersiteId);
                if(player == null)
                {
                    CloseAction?.Invoke();
                    return;
                }

                _playerNoteEntity = new Playernotes
                {
                    Player = _dataService.GetPlayer(PlayerName, PokersiteId),
                    PokersiteId = PokersiteId,
                };
            }

            PlayerNoteEntity.Note = Note;
            _dataService.Store(PlayerNoteEntity);

            CloseAction?.Invoke();
        }

        #endregion
    }
}
