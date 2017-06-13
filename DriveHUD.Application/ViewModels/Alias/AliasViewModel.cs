using DriveHUD.Common.Infrastructure.Base;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Alias
{
    public class AliasViewModel : BaseViewModel
    {
        #region Constructors

        internal AliasViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            _dataService = ServiceLocator.Current.GetInstance<IDataService>();

            PopupModel = new AliasPopupViewModelBase();
            PopupModel.InitializeCommands();

            EditAliasCommand = new RelayCommand(EditAlias);
            RemoveAliasCommand = new RelayCommand(RemoveAlias);

            AliasSorted = new CollectionViewSource();
            AliasSorted.Source = StorageModel.PlayerCollection;
            AliasSorted.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            AddFilter();
        }

        #endregion

        #region Methods

        private void AddAlias(AliasCollectionItem alias)
        {
            if (alias == null)
            {
                return;
            }
            
            StorageModel.PlayerCollection.Add(alias);
            SaveChanges(alias);
            PopupModel.ClosePopup();
        }

        private void SaveChanges(AliasCollectionItem alias)
        {
            _dataService.SaveAlias(alias);
        }

        private void AddFilter()
        {
            AliasSorted.Filter -= new FilterEventHandler(Filter);
            AliasSorted.Filter += new FilterEventHandler(Filter);
        }

        private void Filter(object sender, FilterEventArgs e)
        {
            var src = e.Item as AliasCollectionItem;

            if (src == null)
                e.Accepted = false;
            else if (src.Name != null && !src.Name.ToLower().Contains(SearchFilter.ToLower()))
                e.Accepted = false;
        }

        #endregion

        #region Properties

        private IDataService _dataService;
        private AliasPopupViewModelBase _popupModel;
        private string _searchFilter = string.Empty;

        public AliasPopupViewModelBase PopupModel
        {
            get { return _popupModel; }

            set { SetProperty(ref _popupModel, value); }
        }

        public CollectionViewSource AliasSorted { get; set; }

        public string SearchFilter
        {
            get { return _searchFilter; }
            set
            {
                _searchFilter = value;

                if (!string.IsNullOrEmpty(SearchFilter))
                    AddFilter();

                AliasSorted.View.Refresh();
            }
        }

        #endregion

        #region ICommands

        public ICommand EditAliasCommand { get; set; }
        public ICommand RemoveAliasCommand { get; set; }

        #endregion

        #region ICommands implementation

        private void EditAlias(object obj)
        {
            var viewModelInfo = new AliasViewModelInfo<AliasCollectionItem>()
            {
                Model = obj as AliasCollectionItem,
                Close = PopupModel.ClosePopup,
                Add = AddAlias,
                Save = SaveChanges
            };

            var viewModel = new AliasAddEditViewModel(viewModelInfo);
            PopupModel.OpenPopup(viewModel);
        }

        private void RemoveAlias(object obj)
        {
            if (obj is AliasCollectionItem)
            {
                var alias = obj as AliasCollectionItem;

                StorageModel.PlayerCollection.Remove(alias);
                _dataService.RemoveAlias(alias);
            }
        }

        #endregion
    }
}