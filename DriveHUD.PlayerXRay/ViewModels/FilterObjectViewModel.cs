using DriveHUD.PlayerXRay.DataTypes;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.PlayerXRay.ViewModels
{
    public class FilterObjectViewModel : ReactiveObject
    {
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }

        private FilterEnum filter;


        public FilterEnum Filter
        {
            get
            {
                return filter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref filter, value);
            }
        }

        private double? filterValue;

        public double? Value
        {
            get
            {
                return filterValue;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref filterValue, value);
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isSelected, value);
            }
        }

        private NoteStageType stage;

        public NoteStageType Stage
        {
            get
            {
                return stage;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref stage, value);
            }
        }

        public FilterObjectViewModel Clone()
        {
            return (FilterObjectViewModel)this.MemberwiseClone();
        }
    }
}
