using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Enums;

namespace DriveHUD.Application.ViewModels
{
    public class FilterOmahaHandGridViewModel : FilterViewModel<FilterOmahaHandGridModel>
    {
        internal FilterOmahaHandGridViewModel() : base(EnumViewModelType.FilterOmahaHandGridViewModel)
        {
        }
    }
}
