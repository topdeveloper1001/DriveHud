using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels
{
    public interface IFilterViewModel
    {
        void UpdateDefaultState();

        void RestoreDefaultState();

        object GetDefaultStateModel();

        void UpdateDefaultStateModel(object model);

        void InitializeFilterModel();
    }
}
