using DriveHUD.Application.ViewModels;
using DriveHUD.Entities;
using Model.Enums;
using System.Collections.Generic;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    internal interface ITableConfigurator
    {
        EnumPokerSites Type { get; }

        HudType HudType { get; }

        void ConfigureTable(RadDiagram diagram, HudTableViewModel hudTable, int seats);

        void Update(RadDiagram diagram, HudTableViewModel hudTable);

        IEnumerable<HudElementViewModel> GenerateElements(int seats);
    }
}