//-----------------------------------------------------------------------
// <copyright file="ProfilesViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.Events;
using DriveHUD.PlayerXRay.ViewModels.PopupViewModels;
using DriveHUD.PlayerXRay.Views.PopupViews;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using ReactiveUI;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace DriveHUD.PlayerXRay.ViewModels
{
    public class ProfilesViewModel : WorkspaceViewModel
    {
        private readonly IEventAggregator eventAggregator;

        public ProfilesViewModel()
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            profiles = new ReactiveList<ProfileObject>(NoteService.CurrentNotesAppSettings.Profiles);
            profiles.Changed.Subscribe(x =>
            {
                if (x.Action == NotifyCollectionChangedAction.Add && x.NewItems != null)
                {
                    var addedItems = x.NewItems.OfType<ProfileObject>();

                    if (addedItems != null)
                    {
                        NoteService.CurrentNotesAppSettings.Profiles.AddRange(addedItems);
                    }
                }
                else if (x.Action == NotifyCollectionChangedAction.Remove && x.OldItems != null)
                {
                    var removedItems = x.OldItems.OfType<ProfileObject>();

                    if (removedItems != null)
                    {
                        removedItems.ForEach(p => NoteService.CurrentNotesAppSettings.Profiles.Remove(p));
                    }
                }
            });

            selectedProfileNotes = new ReactiveList<NoteObject>();
            selectedProfileNotes.Changed.Subscribe(x =>
            {
                if (x.Action == NotifyCollectionChangedAction.Add && x.NewItems != null)
                {
                    var addedItems = x.NewItems.OfType<NoteObject>();

                    if (addedItems != null)
                    {
                        addedItems.ForEach(p =>
                        {
                            if (!SelectedProfile.ContainingNotes.Contains(p.ID))
                            {
                                SelectedProfile.ContainingNotes.Add(p.ID);
                            }
                        });
                    }
                }
                else if (x.Action == NotifyCollectionChangedAction.Remove && x.OldItems != null)
                {
                    var removedItems = x.OldItems.OfType<NoteObject>();

                    if (removedItems != null)
                    {
                        removedItems.ForEach(p => SelectedProfile.ContainingNotes.Remove(p.ID));
                    }
                }
            });

            stages = new ReactiveList<StageObject>(NoteService.CurrentNotesAppSettings.StagesList);

            InitializeCommands();
        }

        #region Properties

        private ReactiveList<StageObject> stages;

        public ReactiveList<StageObject> Stages
        {
            get
            {
                return stages;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref stages, value);
            }
        }

        private NoteTreeObjectBase selectedStage;

        public NoteTreeObjectBase SelectedStage
        {
            get
            {
                return selectedStage;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedStage, value);
                SelectedNote = SelectedStage as NoteObject;
            }
        }

        private NoteObject selectedNote;

        public NoteObject SelectedNote
        {
            get
            {
                return selectedNote;
            }
            private set
            {

                this.RaiseAndSetIfChanged(ref selectedNote, value);
            }
        }

        private NoteObject selectedProfileNote;

        public NoteObject SelectedProfileNote
        {
            get
            {
                return selectedProfileNote;
            }
            private set
            {

                this.RaiseAndSetIfChanged(ref selectedProfileNote, value);
            }
        }

        private ReactiveList<NoteObject> selectedProfileNotes;

        public ReactiveList<NoteObject> SelectedProfileNotes
        {
            get
            {
                return selectedProfileNotes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedProfileNotes, value);
            }
        }

        private ReactiveList<ProfileObject> profiles;

        public ReactiveList<ProfileObject> Profiles
        {
            get
            {
                return profiles;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref profiles, value);
            }
        }

        private ProfileObject selectedProfile;

        public ProfileObject SelectedProfile
        {
            get
            {
                return selectedProfile;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedProfile, value);

                selectedProfileNotes.Clear();

                NoteService.CurrentNotesAppSettings.AllNotes.ForEach(x =>
                {
                    if (selectedProfile.ContainingNotes.Contains(x.ID))
                    {
                        selectedProfileNotes.Add(x);
                    }
                });
            }
        }

        public override WorkspaceType WorkspaceType
        {
            get
            {
                return WorkspaceType.Profiles;
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> AddProfileCommand { get; private set; }

        public ReactiveCommand<object> EditProfileCommand { get; private set; }

        public ReactiveCommand<object> RemoveProfileCommand { get; private set; }

        public ReactiveCommand<object> AddToSelectedNotesCommand { get; private set; }

        public ReactiveCommand<object> RemoveFromSelectedNotesCommand { get; private set; }

        #endregion

        private void InitializeCommands()
        {
            AddProfileCommand = ReactiveCommand.Create();
            AddProfileCommand.Subscribe(x => AddProfile());

            var canEdit = this.WhenAny(x => x.SelectedProfile, x => x.Value != null);

            EditProfileCommand = ReactiveCommand.Create(canEdit);
            EditProfileCommand.Subscribe(x => EditProfile());

            RemoveProfileCommand = ReactiveCommand.Create(canEdit);
            RemoveProfileCommand.Subscribe(x => RemoveProfile());

            var canAddToSelectedNotes = this.WhenAny(x => x.SelectedNote, x => x.Value != null);

            AddToSelectedNotesCommand = ReactiveCommand.Create(canAddToSelectedNotes);
            AddToSelectedNotesCommand.Subscribe(x =>
            {
                var existingNote = selectedProfileNotes.FirstOrDefault(p => p.ID == SelectedNote.ID);

                if (existingNote != null)
                {
                    return;
                }

                selectedProfileNotes.Add(SelectedNote);
                NoteService.SaveAppSettings();
            });

            var canRemoveFromSelectedNotes = this.WhenAny(x => x.SelectedProfileNote, x => x.Value != null);

            RemoveFromSelectedNotesCommand = ReactiveCommand.Create(canRemoveFromSelectedNotes);
            RemoveFromSelectedNotesCommand.Subscribe(x =>
            {
                selectedProfileNotes.Remove(SelectedProfileNote);
                NoteService.SaveAppSettings();
            });
        }

        private void AddProfile()
        {
            var addNoteViewModel = new AddEditNoteViewModel();

            addNoteViewModel.OnSaveAction = () =>
            {
                var profile = new ProfileObject
                {
                    Name = addNoteViewModel.Name,
                };

                profiles.Add(profile);

                SelectedProfile = profile;

                NoteService.SaveAppSettings();
            };

            var popupEventArgs = new RaisePopupEventArgs()
            {
                Title = "Add Profile",
                Content = new AddEditNoteView(addNoteViewModel)
            };

            eventAggregator.GetEvent<RaisePopupEvent>().Publish(popupEventArgs);
        }

        private void EditProfile()
        {
            var addNoteViewModel = new AddEditNoteViewModel
            {
                Name = SelectedProfile.Name
            };

            addNoteViewModel.OnSaveAction = () =>
            {
                SelectedProfile.Name = addNoteViewModel.Name;
                NoteService.SaveAppSettings();
            };

            var popupEventArgs = new RaisePopupEventArgs()
            {
                Title = "Edit Profile",
                Content = new AddEditNoteView(addNoteViewModel)
            };

            eventAggregator.GetEvent<RaisePopupEvent>().Publish(popupEventArgs);
        }

        private void RemoveProfile()
        {
            var confirmationViewModel = new YesNoConfirmationViewModel
            {
                ConfirmationMessage = "Are you sure you want to delete the selected item?"
            };

            confirmationViewModel.OnYesAction = () =>
            {
                profiles.Remove(SelectedProfile);
                SelectedProfile = profiles.FirstOrDefault();
                NoteService.SaveAppSettings();
            };

            var popupEventArgs = new RaisePopupEventArgs()
            {
                Title = "Confirm Delete",
                Content = new YesNoConfirmationView(confirmationViewModel)
            };

            eventAggregator.GetEvent<RaisePopupEvent>().Publish(popupEventArgs);
        }
    }
}