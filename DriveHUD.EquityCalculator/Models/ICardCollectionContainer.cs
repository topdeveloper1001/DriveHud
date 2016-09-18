using DriveHUD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Models
{
    internal interface ICardCollectionContainer
    {
        void SetCollection(IEnumerable<CardModel> model);
        void SetRanges(IEnumerable<RangeSelectorItemViewModel> model);
        void Reset();

        int ContainerSize { get; }

        IEnumerable<CardModel> Cards { get; set; }
        IEnumerable<RangeSelectorItemViewModel> Ranges { get; set; }
    }
}
