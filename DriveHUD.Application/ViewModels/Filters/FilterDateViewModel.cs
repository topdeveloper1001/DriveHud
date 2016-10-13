using DriveHUD.Common.Infrastructure.Base;
using Model.Enums;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels
{
    public class FilterDateViewModel : FilterViewModel<FilterDateModel>
    {
        internal FilterDateViewModel(IFilterModelManagerService service) : base(EnumViewModelType.FilteDateViewModel, service)
        {
        }
    }
}
