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
        private string note;

        private long gameNumber;

        private Handnotes handNoteEntity;

        private short pokersiteId;

        private IDataService dataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandNoteViewModel"/> class.
        /// </summary>
        public HandNoteViewModel()
        {
            SaveCommand = new RelayCommand(Save);
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
        }

        public string Note
        {
            get { return note; }
            set
            {
                if (Equals(value, note)) return;

                note = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the game Number.
        /// </summary>
        public long GameNumber
        {
            get
            {
                return this.gameNumber;
            }
            set
            {
                this.gameNumber = value;
                handNoteEntity = dataService.GetHandNote(this.gameNumber, this.pokersiteId);
                if (handNoteEntity != null)
                {
                    Note = handNoteEntity.Note;
                }
            }
        }

        /// <summary>
        /// Gets Hand Note Entity
        /// </summary>
        public Handnotes HandNoteEntity
        {
            get
            {
                return this.handNoteEntity;
            }
        }

        /// <summary>
        /// Gets or sets poker site Id
        /// </summary>
        public short PokersiteId
        {
            get
            {
                return this.pokersiteId;
            }

            set
            {
                pokersiteId = value;
            }
        }

        /// <summary>
        /// Gets or sets the save changes command.
        /// </summary>
        public ICommand SaveCommand { get; set; }

        public Action CloseAction { get; set; }

        /// <summary>
        /// The save.
        /// </summary>
        private void Save()
        {
            if (handNoteEntity == null)
            {
                //Handhistory hh = dataService.GetHandHistory(GameNumber);
                handNoteEntity = new Handnotes
                {
                    Gamenumber = GameNumber,
                    PokersiteId = PokersiteId,
                };
            }

            handNoteEntity.Note = Note;
            dataService.Store(handNoteEntity);
            CloseAction.Invoke();
        }
    }
}