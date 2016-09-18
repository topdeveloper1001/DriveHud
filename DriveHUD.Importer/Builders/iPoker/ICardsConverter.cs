using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.Builders.iPoker
{
    public interface ICardsConverter
    {
        string Convert(string cards);
    }
}