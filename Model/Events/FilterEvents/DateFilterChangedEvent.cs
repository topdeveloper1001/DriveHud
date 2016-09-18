using System;
using Model.Enums;

namespace Model.Events
{
    public class DateFilterChangedEventArgs : EventArgs
    {
        public EnumDateFiter DateFilterType { get; set; }

        public  DateFilterChangedEventArgs(EnumDateFiter dateFilterType)
        {
            this.DateFilterType = dateFilterType;
        }
    }

    public class DateFilterChangedEvent : Prism.Events.PubSubEvent<DateFilterChangedEventArgs>
    {
    }
}
