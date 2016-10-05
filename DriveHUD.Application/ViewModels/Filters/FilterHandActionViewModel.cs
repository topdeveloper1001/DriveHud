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
    public class FilterHandActionViewModel : FilterViewModel<FilterHandActionModel>
    {
        internal FilterHandActionViewModel(IFilterModelManagerService service) : base(EnumViewModelType.FilterHandActionViewModel, service)
        {
        }
    }
}
