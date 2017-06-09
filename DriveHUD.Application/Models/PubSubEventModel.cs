using DriveHUD.Common.Infrastructure.Base;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.Models
{
    public class PubSubMessage
    {
        public EnumPubSubMessageType Type { get; set; }
        public EnumViewModelType SenderType { get; set; }
        public EnumViewModelType RecieverType { get; set; }
        public string Action { get; set; }
        public string Parameter { get; set; }
        public List<KeyValuePair<string, object>> Params { get; set; }
    }
}
