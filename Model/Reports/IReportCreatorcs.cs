using DriveHUD.Entities;
using Model.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model.Reports
{
    public interface IReportCreator
    {
        ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics);
    }
}