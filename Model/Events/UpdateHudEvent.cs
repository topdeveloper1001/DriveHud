using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class UpdateHudEventArgs : EventArgs
    {
        public double Width { get; private set; }
        public double Height { get; private set; }

        public UpdateHudEventArgs(double height, double width)
        {
            this.Width = width;
            this.Height = height;
        }
    }

    public class UpdateHudEvent : PubSubEvent<UpdateHudEventArgs>
    {
    }
}
