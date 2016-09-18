using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Filters
{
    public interface IFilterView
    {
        IFilterViewModel ViewModel { get; }
    }
}
