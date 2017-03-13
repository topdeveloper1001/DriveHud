//-----------------------------------------------------------------------
// <copyright file="AppsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.AppStore;
using DriveHUD.Common.Wpf.Mvvm;
using Model.AppStore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;

namespace DriveHUD.Application.ViewModels
{
    public class AppsViewModel : WindowViewModelBase
    {
        public AppsViewModel()
        {
            Initialize();
        }

        #region Properties     

        private AppStoreType appStoreType;

        public AppStoreType AppStoreType
        {
            get
            {
                return appStoreType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref appStoreType, value);
            }
        }

        private IAppStoreViewModel appStoreViewModel;

        public IAppStoreViewModel AppStoreViewModel
        {
            get
            {
                return appStoreViewModel;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref appStoreViewModel, value);
            }
        }

        private ObservableCollection<AppStorePageViewModel> pages;

        public ObservableCollection<AppStorePageViewModel> Pages
        {
            get
            {
                return pages;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref pages, value);
            }
        }

        private AppStorePageViewModel currentPage;

        public AppStorePageViewModel CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                if (currentPage != null)
                {
                    currentPage.IsSelected = false;
                }

                this.RaiseAndSetIfChanged(ref currentPage, value);

                if (currentPage != null)
                {
                    currentPage.IsSelected = true;
                }
            }
        }

        public bool IsPagesVisible
        {
            get
            {
                return pages != null && pages.Count > 1;
            }
        }

        private string searchText;

        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref searchText, value);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> SelectPageCommand { get; private set; }

        public ReactiveCommand<object> NextPageCommand { get; private set; }

        public ReactiveCommand<object> PreviousPageCommand { get; private set; }

        public ReactiveCommand<object> SearchCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void Initialize()
        {
            Pages = new ObservableCollection<AppStorePageViewModel>();
            InitializeObservables();
            InitializeCommands();
            Load();
        }

        private void InitializeObservables()
        {
            this.ObservableForProperty(x => x.AppStoreType).Subscribe(x => Load());
            this.ObservableForProperty(x => x.CurrentPage).Subscribe(x =>
            {
                if (AppStoreViewModel != null && CurrentPage != null)
                {
                    AppStoreViewModel.Refresh(CurrentPage.PageNumber);
                }
            });

            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => Pages.CollectionChanged += h,
                h => Pages.CollectionChanged -= h).Subscribe(x =>
                {
                    this.RaisePropertyChanged(nameof(IsPagesVisible));
                });
        }

        private void InitializeCommands()
        {
            SelectPageCommand = ReactiveCommand.Create();
            SelectPageCommand.Subscribe(x =>
            {
                var selectedPage = x as AppStorePageViewModel;

                if (selectedPage != null)
                {
                    CurrentPage = selectedPage;
                }
            });

            PreviousPageCommand = ReactiveCommand.Create();
            PreviousPageCommand.Subscribe(x =>
            {
                if (CurrentPage != null)
                {
                    SetPage(CurrentPage.PageNumber - 1);
                }
            });

            NextPageCommand = ReactiveCommand.Create();
            NextPageCommand.Subscribe(x =>
            {
                if (CurrentPage != null)
                {
                    SetPage(CurrentPage.PageNumber + 1);
                }
            });

            SearchCommand = ReactiveCommand.Create();
            SearchCommand.Subscribe(x =>
            {
                if (AppStoreViewModel != null)
                {
                    AppStoreViewModel.Search(SearchText);
                }
            });
        }

        // to do: replace with service locator
        private void Load()
        {
            if (AppStoreViewModel != null)
            {
                AppStoreViewModel.Updated -= OnAppStoreViewModelUpdated;
            }

            Pages.Clear();

            switch (appStoreType)
            {
                case AppStoreType.Recommended:
                    AppStoreViewModel = new ProductAppStoreViewModel();
                    AppStoreViewModel.Updated += OnAppStoreViewModelUpdated;
                    AppStoreViewModel.Initialize();
                    break;

                case AppStoreType.Training:
                    AppStoreViewModel = new TrainingAppStoreViewModel();
                    AppStoreViewModel.Updated += OnAppStoreViewModelUpdated;
                    AppStoreViewModel.Initialize();
                    break;

                default:
                    AppStoreViewModel = new EmptyAppStoreViewModel();
                    break;
            }

            RefreshPages();
        }

        private void OnAppStoreViewModelUpdated(object sender, EventArgs e)
        {
            RefreshPages();
        }

        private void RefreshPages()
        {
            var pagesCount = AppStoreViewModel.ProductsPerPage != 0 ? (int)Math.Ceiling((decimal)AppStoreViewModel.ItemsCount / AppStoreViewModel.ProductsPerPage) : 0;

            Pages.Clear();

            if (pagesCount > 0)
            {
                Pages.AddRange(Enumerable.Range(1, pagesCount).Select(x => new AppStorePageViewModel(x)));
            }

            CurrentPage = Pages.FirstOrDefault();
        }

        private void SetPage(int pageNumber)
        {
            if (pageNumber < 1 || pageNumber > Pages.Count)
            {
                return;
            }

            var selectedPage = Pages.FirstOrDefault(x => x.PageNumber == pageNumber);

            if (selectedPage != null)
            {
                CurrentPage = selectedPage;
            }
        }

        #endregion
    }
}