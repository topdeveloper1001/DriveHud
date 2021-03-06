﻿//-----------------------------------------------------------------------
// <copyright file="PlayerNoteViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Data;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Popups
{
    public class PlayerNoteViewModel : BaseViewModel
    {
        private readonly IDataService dataService = ServiceLocator.Current.GetInstance<IDataService>();

        private List<Playernotes> playerNoteEntities;

        private string initialNotes = string.Empty;

        private readonly Players player;

        public PlayerNoteViewModel(int playerId)
        {
            player = dataService.GetPlayer(playerId);
            Initialize();
        }

        public PlayerNoteViewModel(short pokerSiteId, string playerName)
        {
            player = dataService.GetPlayer(playerName, pokerSiteId);
            Initialize();
        }

        private void Initialize()
        {
            SaveCommand = new RelayCommand(Save);
            Note = string.Empty;
            playerNoteEntities = new List<Playernotes>();
            LoadNote();
        }

        private void LoadNote()
        {
            playerNoteEntities = player != null ?
                dataService.GetPlayerNotes(player.PlayerId).ToList() :
                null;

            if (playerNoteEntities != null)
            {
                Note = NoteBuilder.BuildNote(playerNoteEntities);
                initialNotes = Note;
            }
        }

        #region Properties       

        private string note;

        public string Note
        {
            get { return note; }
            set { SetProperty(ref note, value); }
        }

        public bool HasNotes
        {
            get
            {
                return playerNoteEntities != null && playerNoteEntities.Count > 0;
            }
        }

        public Action CloseAction { get; set; }

        #endregion

        #region ICommand

        public ICommand SaveCommand { get; set; }

        #endregion

        #region ICommand implementation

        private void Save()
        {
            if (!HasNotes)
            {
                if (player == null)
                {
                    CloseAction?.Invoke();
                    return;
                }

                // check if notes exist because notes might be added on another table
                var notes = dataService.GetPlayerNotes(player.PlayerId);

                var manualNote = notes.FirstOrDefault(x => !x.IsAutoNote);

                if (manualNote == null)
                {
                    manualNote = new Playernotes
                    {
                        PlayerId = player.PlayerId,
                        PokersiteId = player.PokersiteId,
                        Timestamp = DateTime.UtcNow
                    };

                    playerNoteEntities.Add(manualNote);
                }

                manualNote.Note = Note;

                dataService.Store(manualNote);

                CloseAction?.Invoke();

                return;
            }

            // no auto
            if (playerNoteEntities.All(x => !x.IsAutoNote))
            {
                if (string.IsNullOrEmpty(Note))
                {
                    dataService.DeletePlayerNotes(playerNoteEntities);
                    playerNoteEntities.Clear();
                }
                else
                {
                    var manualNote = playerNoteEntities.FirstOrDefault();
                    manualNote.Note = Note;

                    dataService.Store(manualNote);
                }

                CloseAction?.Invoke();

                return;
            }

            var newNotes = NoteBuilder.ParseNotes(Note);
            var manualNoteText = newNotes.Item1;

            // no auto
            if (string.IsNullOrEmpty(newNotes.Item2))
            {
                // delete auto notes if exists
                var notesToDelete = playerNoteEntities.Where(x => x.IsAutoNote).ToArray();
                dataService.DeletePlayerNotes(notesToDelete);

                notesToDelete.ForEach(x => playerNoteEntities.Remove(x));
            }
            else
            {
                // check if any auto notes
                if (playerNoteEntities.Any(x => x.IsAutoNote))
                {
                    var oldNotes = NoteBuilder.ParseNotes(initialNotes);

                    // if auto note were changed, then make them all manual
                    if (newNotes.Item2 != oldNotes.Item2)
                    {
                        // delete all auto notes because we move them to manual
                        var notesToDelete = playerNoteEntities.Where(x => x.IsAutoNote).ToArray();
                        dataService.DeletePlayerNotes(notesToDelete);
                        notesToDelete.ForEach(x => playerNoteEntities.Remove(x));

                        manualNoteText = Note;
                    }
                }
                else
                {
                    manualNoteText = Note;
                }
            }

            // no manual
            if (string.IsNullOrEmpty(manualNoteText))
            {
                // delete manual notes if exists
                var notesToDelete = playerNoteEntities.Where(x => !x.IsAutoNote).ToArray();
                dataService.DeletePlayerNotes(notesToDelete);
                notesToDelete.ForEach(x => playerNoteEntities.Remove(x));
            }
            else
            {
                var manualNote = playerNoteEntities.FirstOrDefault(x => !x.IsAutoNote);

                if (manualNote == null)
                {
                    if (player == null)
                    {
                        CloseAction?.Invoke();
                        return;
                    }

                    manualNote = new Playernotes
                    {
                        PlayerId = player.PlayerId,
                        PokersiteId = player.PokersiteId,
                        Timestamp = DateTime.UtcNow
                    };

                    playerNoteEntities.Add(manualNote);
                }

                manualNote.Note = manualNoteText;

                dataService.Store(manualNote);
            }


            CloseAction?.Invoke();
        }

        #endregion
    }
}
