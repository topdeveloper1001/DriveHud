using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace AcePokerSolutions.DataAccessHelper.DriveHUD
{
    [ProtoContract] 
    public class StatDto
    {
        public StatDto()
        {
        }

        public StatDto(int? occurred, int? couldOccurred)
        {
            Occurred = occurred.HasValue ? occurred.Value : 0;
            CouldOccurred = couldOccurred.HasValue ? couldOccurred.Value : 0;
        }

        [ProtoMember(1)]
        public virtual decimal Value { get; set; }

        [ProtoMember(2)]
        public virtual int Occurred { get; set; }

        [ProtoMember(3)]
        public virtual int CouldOccurred { get; set; }

        [ProtoMember(4)]
        public string CardRange { get; set; }
    }
}
