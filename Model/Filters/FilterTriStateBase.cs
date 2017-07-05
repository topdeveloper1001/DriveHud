using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Model.Filters
{
    [Serializable]
    public class FilterTriStateBase : FilterBaseEntity
    {
        public FilterTriStateBase() : this(EnumTriState.Any) { }

        public FilterTriStateBase(EnumTriState param = EnumTriState.Any)
        {
            CurrentTriState = param;
        }
     
        protected EnumTriState currentTriState;

        public virtual EnumTriState CurrentTriState
        {
            get { return currentTriState; }
            set
            {
                if (value == currentTriState)
                {
                    return;
                }

                currentTriState = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Swap to the next defined TriState
        /// </summary>
        public void TriStateSwap()
        {
            if (CurrentTriState == EnumTriState.Any)
            {
                CurrentTriState = EnumTriState.On;
            }
            else if (CurrentTriState == EnumTriState.On)
            {
                CurrentTriState = EnumTriState.Off;
            }
            else
            {
                CurrentTriState = EnumTriState.Any;
            }
        }
    }
}