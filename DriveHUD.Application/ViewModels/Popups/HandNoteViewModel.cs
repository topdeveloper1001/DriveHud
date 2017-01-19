using System;
using System.Windows.Input;
using DriveHUD.Entities;
using Model.Enums;
using Model.Interfaces;
using DriveHUD.Common.Infrastructure.Base;
using Microsoft.Practices.ServiceLocation;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// The hand note view model.
    /// </summary>
    public class HandNoteViewModel : BaseViewModel
    {
        private string _note;
        private long _gameNumber;
        private short _pokerSiteId;
        private Handnotes _handNoteEntity;

        private IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandNoteViewModel"/> class.
        /// </summary>
        public HandNoteViewModel(long gameNumber, short pokerSiteId)
        {
            SaveCommand = new RelayCommand(Save);
            _dataService = ServiceLocator.Current.GetInstance<IDataService>();

            this._gameNumber = gameNumber;
            this._pokerSiteId = pokerSiteId;

            this.Note = LoadHandNoteViewModel(_gameNumber, pokerSiteId);
        }

        public string Note
        {
            get { return _note; }
            set
            {
                if (Equals(value, _note)) return;

                _note = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets Hand Note Entity
        /// </summary>
        public Handnotes HandNoteEntity
        {
            get
            {
                return this._handNoteEntity;
            }
        }

        /// <summary>
        /// Gets or sets the save changes command.
        /// </summary>
        public ICommand SaveCommand { get; set; }

        public Action CloseAction { get; set; }

        private string LoadHandNoteViewModel(long gameNumber, short pokerSiteId)
        {
            _handNoteEntity = _dataService.GetHandNote(gameNumber, pokerSiteId);
            if (_handNoteEntity != null)
            {
                return _handNoteEntity.Note;
            }

            return string.Empty;
        }

        /// <summary>
        /// The save.
        /// </summary>
        private void Save()
        {
            if (_handNoteEntity == null)
            {
                _handNoteEntity = new Handnotes
                {
                    Gamenumber = this._gameNumber,
                    PokersiteId = this._pokerSiteId,
                };
            }

            _handNoteEntity.Note = Note;
            _dataService.Store(_handNoteEntity);
            CloseAction.Invoke();
        }
    }
}