using HandHistories.Objects.Actions;

namespace Model
{
    /// <summary>
    /// Same as Condition but extended with player responce ti particular situtation
    /// </summary>
    public class ConditionalBet : Condition
    {
        public bool Faced { get; set; }
        public bool Folded { get; set; }
        public bool Called { get; set; }
        public bool Raised { get; set; }

        public bool CheckAction(HandAction action)
        {
            Faced = true;
            if (action.HandActionType == HandActionType.FOLD)
            {
               Folded = true;
               return true;
            }
            if (action.IsCall())
            {
               Called = true;
               return true;
            }
            if (action.IsRaise())
            {
                Raised = true;
                return true;
            }

            return false;
        }
    }
}