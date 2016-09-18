using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model.Enums;
using DriveHUD.Application.Models;

using Prism.Interactivity.InteractionRequest;
using DriveHUD.Common.Infrastructure.Base;
using Model;
using Model.Filters;

namespace DriveHUD.Application.ViewModels.PopupContainers.Notifications
{
    public class PopupContainerFiltersViewModelNotification : Confirmation
    {
        public PopupContainerFiltersViewModelNotification()
        {
        }

        public PopupContainerFiltersViewModelNotification(object items)
            : this()
        {
        }

        public FilterTuple FilterTuple { get; set; }
    }
}

