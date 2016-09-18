using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Replayer
{
    public class ChipModel
    {
        private int _count;
        private EnumChipColor _chipColor;

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public EnumChipColor ChipColor
        {
            get { return _chipColor; }
            set { _chipColor = value; }
        }

    }
}
