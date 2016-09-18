using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal interface IPokerEvaluator
    {
        void SetCardsOnTable(string cards);

        void SetPlayerCards(int seat, string cards);

        IEnumerable<int> GetWinners();        
    }
}