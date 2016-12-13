using System;
using System.Linq.Expressions;
using DriveHUD.Entities;
using Prism.Events;

namespace Model.Events.FilterEvents
{
    public class UpdateReportEvent : PubSubEvent<Expression<Func<Playerstatistic, bool>>>
    {
    }
}
