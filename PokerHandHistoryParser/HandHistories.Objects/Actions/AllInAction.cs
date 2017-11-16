using System.Runtime.Serialization;
using HandHistories.Objects.Cards;
using System;
using System.Xml.Serialization;

namespace HandHistories.Objects.Actions
{
    [Serializable]
    public class AllInAction : HandAction
    {
        [XmlAttribute]
        public bool IsRaiseAllIn { get; set; }

        [XmlAttribute]
        public HandActionType SourceActionType { get; set; }

        public AllInAction()
        {
        }

        public AllInAction(string playerName,
                           decimal amount,
                           Street street,
                           bool isRaiseAllIn,
                           int actionNumber = 0)
            : base(playerName, HandActionType.ALL_IN, amount, street, true, actionNumber)
        {
            IsRaiseAllIn = isRaiseAllIn;
            SourceActionType = HandActionType.BET;
        }

        public AllInAction(string playerName,
                          decimal amount,
                          Street street,
                          bool isRaiseAllIn,
                          HandActionType actionType,
                          int actionNumber = 0)
            : base(playerName, HandActionType.ALL_IN, amount, street, true, actionNumber)
        {
            IsRaiseAllIn = isRaiseAllIn;
            SourceActionType = actionType;
        }

        public override string ToString()
        {
            return base.ToString() + "-RaiseAllIn=" + IsRaiseAllIn;
        }

    }
}