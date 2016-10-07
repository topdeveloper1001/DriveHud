using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Common.Utils
{
    [Serializable]
    public class SerializableTuple<T1, T2>
    {
        public SerializableTuple() { }

        public SerializableTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public static implicit operator SerializableTuple<T1, T2>(Tuple<T1, T2> t)
        {
            return new SerializableTuple<T1, T2>()
            {
                Item1 = t.Item1,
                Item2 = t.Item2
            };
        }

        public static implicit operator Tuple<T1, T2>(SerializableTuple<T1, T2> t)
        {
            return Tuple.Create(t.Item1, t.Item2);
        }
    }
}
