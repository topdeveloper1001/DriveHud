using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.EquityCalculator.Models
{
    public class BoardModel : CardCollectionContainer
    {
        #region Fields
        private readonly int _boardSize = 5;
        #endregion

        #region Properties
        public override int ContainerSize
        {
            get
            {
                return _boardSize;
            }
        }
        #endregion

        public BoardModel() : base() { }
        public BoardModel(IEnumerable<CardModel> model) : base(model) { }

        public override string ToString()
        {
            if (Cards == null || Cards.Count() == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            foreach(var card in this.Cards)
            {
                builder.Append(card.ToString());
            }

            return builder.ToString();
        }
    }
}
