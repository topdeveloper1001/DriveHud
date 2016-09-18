using Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Filters
{
    public class FilterTuple : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public EnumFilterModelType ModelType { get; set; }
        public EnumViewModelType ViewModelType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
