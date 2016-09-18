using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    /// <summary>
    /// Extension for situation checks : Raise Situation, Bet Situation, Call Situation 
    /// </summary>
    public static class HandActionExtension
    {
        /// <summary>
        /// Check if action is raise action
        /// </summary>
        /// <param name="handAction">Hand action</param>
        /// <returns>True - if action is raise, otherwise false</returns>
        public static bool IsRaise(this HandAction handAction)
        {
            if (handAction.HandActionType == HandActionType.RAISE)
            {
                return true;
            }

            if (handAction.HandActionType == HandActionType.ALL_IN)
            {
                return ((AllInAction)handAction).SourceActionType == HandActionType.RAISE;
            }

            return false;
        }

        /// <summary>
        /// Check if action is bet action
        /// </summary>
        /// <param name="handAction">Hand action</param>
        /// <returns>True - if action is bet, otherwise false</returns>
        public static bool IsBet(this HandAction handAction)
        {
            if (handAction.HandActionType == HandActionType.BET)
            {
                return true;
            }

            if (handAction.HandActionType == HandActionType.ALL_IN)
            {
                return ((AllInAction)handAction).SourceActionType == HandActionType.BET;
            }

            return false;
        }

        /// <summary>
        /// Check if action is raise call
        /// </summary>
        /// <param name="handAction">Hand call</param>
        /// <returns>True - if call is raise, otherwise false</returns>
        public static bool IsCall(this HandAction handAction)
        {
            if (handAction.HandActionType == HandActionType.CALL)
            {
                return true;
            }

            if (handAction.HandActionType == HandActionType.ALL_IN)
            {
                return ((AllInAction)handAction).SourceActionType == HandActionType.CALL;
            }

            return false;
        }

        /// <summary>
        /// Filter preflop by predicate function
        /// </summary>
        /// <param name="handActions">Hand actions</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns></returns>
        public static IEnumerable<HandAction> PreFlopWhere(this IEnumerable<HandAction> handActions, Func<HandAction, bool> predicate)
        {
            return handActions.Where(x => x.Street == Street.Preflop && predicate(x));
        }

        /// <summary>
        /// Filter flop by predicate function
        /// </summary>
        /// <param name="handActions">Hand actions</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns></returns>
        public static IEnumerable<HandAction> FlopWhere(this IEnumerable<HandAction> handActions, Func<HandAction, bool> predicate)
        {
            return handActions.Where(x => x.Street == Street.Flop && predicate(x));
        }

        /// <summary>
        /// Filter turn by predicate function
        /// </summary>
        /// <param name="handActions">Hand actions</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns></returns>
        public static IEnumerable<HandAction> TurnWhere(this IEnumerable<HandAction> handActions, Func<HandAction, bool> predicate)
        {
            return handActions.Where(x => x.Street == Street.Turn && predicate(x));
        }

        /// <summary>
        /// Filter river by predicate function
        /// </summary>
        /// <param name="handActions">Hand actions</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns></returns>
        public static IEnumerable<HandAction> RiverWhere(this IEnumerable<HandAction> handActions, Func<HandAction, bool> predicate)
        {
            return handActions.Where(x => x.Street == Street.River && predicate(x));
        }

        /// <summary>
        /// Determines whenever any hand action in preflop satisfies a condition
        /// </summary>
        /// <param name="handActions">Hand actions</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns></returns>
        public static bool PreFlopAny(this IEnumerable<HandAction> handActions, Func<HandAction, bool> predicate)
        {
            return handActions.Any(x => x.Street == Street.Preflop && predicate(x));
        }

        /// <summary>
        /// Determines whenever any hand action in flop satisfies a condition
        /// </summary>
        /// <param name="handActions">Hand actions</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns></returns>
        public static bool FlopAny(this IEnumerable<HandAction> handActions, Func<HandAction, bool> predicate)
        {
            return handActions.Any(x => x.Street == Street.Flop && predicate(x));
        }

        /// <summary>
        /// Determines whenever any hand action in turn satisfies a condition
        /// </summary>
        /// <param name="handActions">Hand actions</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns></returns>
        public static bool TurnAny(this IEnumerable<HandAction> handActions, Func<HandAction, bool> predicate)
        {
            return handActions.Any(x => x.Street == Street.Turn && predicate(x));
        }

        /// <summary>
        /// Determines whenever any hand action in river satisfies a condition
        /// </summary>
        /// <param name="handActions">Hand actions</param>
        /// <param name="predicate">Predicate function</param>
        /// <returns></returns>
        public static bool RiverAny(this IEnumerable<HandAction> handActions, Func<HandAction, bool> predicate)
        {
            return handActions.Any(x => x.Street == Street.River && predicate(x));
        }
    }
}