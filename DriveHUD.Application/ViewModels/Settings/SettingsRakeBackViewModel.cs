using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using Model.Settings;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsRakeBackViewModel : SettingsViewModel<RakeBackSettingsModel>
    {
        #region Constructor

        internal SettingsRakeBackViewModel(string name) : base(name)
        {
            Initialize();
        }

        private void Initialize()
        {
            PopupModel = new SettingsPopupViewModelBase();
            PopupModel.InitializeCommands();

            EditRakeBackCommand = new RelayCommand(EditRakeBack);
            EditBonusCommand = new RelayCommand(EditBonus);

            RemoveRakeBackCommand = new RelayCommand(RemoveRakeBack);
            RemoveBonusCommand = new RelayCommand(RemoveBonus);

            BonusesList = new ObservableCollection<BonusModel>();
            RakeBackList = new ObservableCollection<RakeBackModel>();
        }

        private void BonusesList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    SettingsModel.BonusesList.AddRange(e.NewItems.Cast<BonusModel>());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    e.OldItems.Cast<BonusModel>().ForEach(x => SettingsModel.BonusesList.Remove(x));
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void RakeBackList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    SettingsModel.RakeBackList.AddRange(e.NewItems.Cast<RakeBackModel>());
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    e.OldItems.Cast<RakeBackModel>().ForEach(x => SettingsModel.RakeBackList.Remove(x));
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        public override void SetSettingsModel(ISettingsBase model)
        {
            SetSettingsModelInternal(model);
        }

        private void SetSettingsModelInternal(ISettingsBase model)
        {
            base.SetSettingsModel(model);

            BonusesList.CollectionChanged -= BonusesList_CollectionChanged;
            RakeBackList.CollectionChanged -= RakeBackList_CollectionChanged;

            BonusesList.Clear();
            RakeBackList.Clear();

            if (SettingsModel?.BonusesList != null)
            {
                BonusesList.AddRange(SettingsModel.BonusesList);
            }

            if (SettingsModel?.RakeBackList != null)
            {
                RakeBackList.AddRange(SettingsModel.RakeBackList);
            }

            BonusesList.CollectionChanged += BonusesList_CollectionChanged;
            RakeBackList.CollectionChanged += RakeBackList_CollectionChanged;
        }

        #endregion

        #region Methods

        private void AddBonus(BonusModel bonus)
        {
            if (bonus == null)
            {
                return;
            }

            BonusesList.Add(bonus);
            PopupModel.ClosePopup();
        }

        private void AddRakeBack(RakeBackModel rakeBack)
        {
            if (rakeBack == null)
            {
                return;
            }

            RakeBackList.Add(rakeBack);
            PopupModel.ClosePopup();
        }

        #endregion

        #region Properties

        private SettingsPopupViewModelBase _popupModel;
        private ObservableCollection<BonusModel> _bonusesList;
        private ObservableCollection<RakeBackModel> _rakeBackList;

        public ObservableCollection<RakeBackModel> RakeBackList
        {
            get { return _rakeBackList; }
            set { SetProperty(ref _rakeBackList, value); }
        }

        public ObservableCollection<BonusModel> BonusesList
        {
            get { return _bonusesList; }
            set
            {
                SetProperty(ref _bonusesList, value);
            }
        }

        public SettingsPopupViewModelBase PopupModel
        {
            get
            {
                return _popupModel;
            }

            set
            {
                SetProperty(ref _popupModel, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand EditRakeBackCommand { get; set; }
        public ICommand EditBonusCommand { get; set; }
        public ICommand RemoveRakeBackCommand { get; set; }
        public ICommand RemoveBonusCommand { get; set; }

        #endregion

        #region ICommand Implementation

        private void EditRakeBack(object obj)
        {
            var viewModelInfo = new SettingsRakeBackViewModelInfo<RakeBackModel>
            {
                Model = (obj as RakeBackModel),
                Close = PopupModel.ClosePopup,
                Add = AddRakeBack
            };

            var viewModel = new SettingsRakeBackAddEditViewModel(viewModelInfo);
            PopupModel.OpenPopup(viewModel);
        }

        private void EditBonus(object obj)
        {
            var viewModelInfo = new SettingsRakeBackViewModelInfo<BonusModel>
            {
                Model = (obj as BonusModel),
                Close = PopupModel.ClosePopup,
                Add = AddBonus
            };

            var viewModel = new SettingsBonusesAddEditViewModel(viewModelInfo);
            PopupModel.OpenPopup(viewModel);
        }

        private void RemoveRakeBack(object obj)
        {
            if(obj is RakeBackModel)
            {
                RakeBackList.Remove(obj as RakeBackModel);
            }
        }

        private void RemoveBonus(object obj)
        {
            if(obj is BonusModel)
            {
                BonusesList.Remove(obj as BonusModel);
            }
        }
        #endregion
    }
}
