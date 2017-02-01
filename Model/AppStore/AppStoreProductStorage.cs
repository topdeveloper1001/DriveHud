using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.AppStore
{
    [Serializable]
    public class AppStoreProductStorage
    {
        public List<AppStoreProduct> Products { get; set; }
    }
}