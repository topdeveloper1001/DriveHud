﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class UpdateFilterRequestEventArgs : EventArgs
    {

    }

    public class UpdateFilterRequestEvent : PubSubEvent<UpdateFilterRequestEventArgs>
    {

    }
}
