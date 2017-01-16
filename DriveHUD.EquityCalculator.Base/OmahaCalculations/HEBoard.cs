using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Base.OmahaCalculations
{
    /// <summary>
    ///  methods for getting complete hold'em/omaha boards, either* randomly or with combinatorial enumeration
    /// </summary>
    internal abstract class HEBoard
    {
        /// <summary>
        /// starting board, never changes
        /// </summary>
        internal String[] current;
        /// <summary>
        /// remaining cards in deck, never changes
        /// </summary>
        internal String[] deck;
        /// <summary>
        /// next board after call to next()
        /// </summary>
        internal String[] board = new String[5];
        internal HEBoard(String[] deck, String[] current)
        {
            this.deck = deck;
            this.current = current;
        }
        /// <summary>
        /// how many boards are there
        /// </summary>
        /// <returns></returns>
        internal abstract int Count();
        /// <summary>
        /// create the next board
        /// </summary>
        internal abstract void Next();
        /// <summary>
        /// how many cards will be picked
        /// </summary>
        /// <returns></returns>
        internal abstract int Pick();
        /// <summary>
        /// is this an exact enumeration
        /// </summary>
        /// <returns></returns>
        internal abstract bool Exact();
    }

}
