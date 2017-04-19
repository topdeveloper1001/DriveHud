using System;
using Model.Enums;

namespace Model.Events
{
	public class DateFilterChangedEventArgs : EventArgs
	{
		public EnumDateFiterStruct DateFilterType { get; set; }

		public DateFilterChangedEventArgs(EnumDateFiterStruct dateFilterType)
		{
			this.DateFilterType = dateFilterType;
		}
	}

	public class DateFilterChangedEvent : Prism.Events.PubSubEvent<DateFilterChangedEventArgs>
    {
    }
}
