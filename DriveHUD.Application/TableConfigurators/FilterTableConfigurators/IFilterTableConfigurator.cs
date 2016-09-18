using DriveHUD.Application.ViewModels;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    public interface IFilterTableConfigurator
    {
        void ConfigureTable(RadDiagram diagram, FilterStandardViewModel viewModel, int seats);
    }
}
