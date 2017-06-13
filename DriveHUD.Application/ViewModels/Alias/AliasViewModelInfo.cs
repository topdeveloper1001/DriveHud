using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Alias
{
    public class AliasViewModelInfo<T>
    {
        internal T Model { get; set; }

        internal Action<T> Add { get; set; }
        internal Action<T> Save { get; set; }
        internal Action Close { get; set; }
    }
}
