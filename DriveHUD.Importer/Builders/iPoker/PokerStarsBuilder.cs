using DriveHUD.Importers.Bovada;
using Model.Site;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.Builders.iPoker
{
    /// <summary>
    /// PokerStars hand history builder
    /// </summary>
    internal class PokerStarsBuilder : IHandHistoryBuilder
    {
        public string Build(HandModel2 handModel, IPokerTable tableModel, ISiteConfiguration configuration, out Game game)
        {
            throw new NotImplementedException();
        }
    }
}