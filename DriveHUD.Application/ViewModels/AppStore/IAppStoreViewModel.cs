using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public interface IAppStoreViewModel : INotifyPropertyChanged
    {
        void Initialize();
    }
}
