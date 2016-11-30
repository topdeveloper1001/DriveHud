using DriveHUD.HUD.Service;
using Prism.Events;
using System;

namespace Model.Events.HudEvents
{
    public class HudCommandEventArgs : EventArgs
    {
        public int WindowId { get; private set; }
        public EnumCommand HudCommand { get; private set; }
        public object CommandObject { get; private set; }

        public HudCommandEventArgs(int windowId, EnumCommand command)
            : this(windowId, command, null)
        { }

        public HudCommandEventArgs(int windowId, EnumCommand command, object obj)
        {
            this.WindowId = windowId;
            this.HudCommand = command;
            this.CommandObject = obj;
        }
    }

    public class HudCommandEvent : PubSubEvent<HudCommandEventArgs>
    {
    }
}
