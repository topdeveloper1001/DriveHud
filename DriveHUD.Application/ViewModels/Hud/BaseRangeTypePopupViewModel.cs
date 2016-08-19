using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels
{
    public abstract class BaseRangeTypePopupViewModel : ViewModelBase
    {
        public BaseRangeTypePopupViewModel()
        {
            InitializeCommands();
        }

        protected virtual void InitializeCommands()
        {
            SaveCommand = ReactiveCommand.Create(CanSave());
            SaveCommand.Subscribe(x => Save());

            CreateCommand = ReactiveCommand.Create();
            CreateCommand.Subscribe(x => Create());
        }

        protected abstract IObservable<bool> CanSave();

        protected abstract void Save();

        protected abstract void Create();

        #region Commands

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CreateCommand { get; private set; }

        #endregion
    }
}
