//-----------------------------------------------------------------------
// <copyright file="ActionSettings.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ReactiveUI;
using System.Collections.Generic;

namespace DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.ActionsObjects
{
    public class ActionSettings : ReactiveObject
    {
        private ActionTypeEnum firstType;

        public ActionTypeEnum FirstType
        {
            get
            {
                return firstType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref firstType, value);
            }
        }

        public double FirstMinValue { get; set; }

        public double FirstMaxValue { get; set; }

        private ActionTypeEnum secondType;

        public ActionTypeEnum SecondType
        {
            get
            {
                return secondType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref secondType, value);
            }
        }

        public double SecondMinValue { get; set; }

        public double SecondMaxValue { get; set; }

        private ActionTypeEnum thirdType;

        public ActionTypeEnum ThirdType
        {
            get
            {
                return thirdType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref thirdType, value);
            }
        }

        public double ThirdMinValue { get; set; }

        public double ThirdMaxValue { get; set; }

        private ActionTypeEnum fourthType;

        public ActionTypeEnum FourthType
        {
            get
            {
                return fourthType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref fourthType, value);
            }
        }

        public double FourthMinValue { get; set; }

        public double FourthMaxValue { get; set; }

        public bool Equals(ActionSettings x2)
        {
            ActionSettings x1 = this;

            return x1.FirstMaxValue == x2.FirstMaxValue &&
                   x1.FirstMinValue == x2.FirstMinValue &&
                   x1.FirstType == x2.FirstType &&
                   x1.SecondMaxValue == x2.SecondMaxValue &&
                   x1.SecondMinValue == x2.SecondMinValue &&
                   x1.SecondType == x2.SecondType &&
                   x1.ThirdMaxValue == x2.ThirdMaxValue &&
                   x1.ThirdMinValue == x2.ThirdMinValue &&
                   x1.ThirdType == x2.ThirdType &&
                   x1.FourthMaxValue == x2.FourthMaxValue &&
                   x1.FourthMinValue == x2.FourthMinValue &&
                   x1.FourthType == x2.FourthType;
        }

        public IEnumerable<long> GetActionValue()
        {
            if (FirstType == ActionTypeEnum.Any &&
                SecondType == ActionTypeEnum.Any &&
                ThirdType == ActionTypeEnum.Any &&
                FourthType == ActionTypeEnum.Any)
                return null;

            var result = new List<long>();

            var values = new List<string>();

            switch (FirstType)
            {
                case ActionTypeEnum.Bet:
                    values.Add("Bet");
                    break;

                case ActionTypeEnum.Call:
                    values.Add("Call");
                    break;

                case ActionTypeEnum.Raise:
                    values.Add("Raise");
                    break;

                case ActionTypeEnum.Check:
                    values.Add("Check");
                    break;

                case ActionTypeEnum.Fold:
                    values.Add("Fold");
                    return result;

                case ActionTypeEnum.Any:
                    values.Add("Bet");
                    values.Add("Call");
                    values.Add("Raise");
                    values.Add("Check");
                    values.Add("Fold");
                    break;
            }

            switch (SecondType)
            {
                case ActionTypeEnum.Call:
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i] += "Call";
                    }

                    break;

                case ActionTypeEnum.Raise:
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i] += "Raise";
                    }
                    break;


                case ActionTypeEnum.Fold:
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i] += "Fold";
                    }

                    return result;

                case ActionTypeEnum.Any:
                    if (ThirdType == ActionTypeEnum.Any &&
                        FourthType == ActionTypeEnum.Any)
                        break;

                    List<string> tempList = new List<string>(values);
                    values.Clear();

                    foreach (string val in tempList)
                    {
                        values.Add(val + "Call");
                        values.Add(val + "Raise");
                        values.Add(val + "Fold");
                    }

                    break;
            }

            switch (ThirdType)
            {
                case ActionTypeEnum.Call:
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i] += "Call";
                    }

                    break;

                case ActionTypeEnum.Raise:
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i] += "Raise";
                    }

                    break;

                case ActionTypeEnum.Fold:
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i] += "Fold";
                    }

                    return result;

                case ActionTypeEnum.Any:
                    if (FourthType == ActionTypeEnum.Any)
                        break;

                    List<string> tempList = new List<string>(values);
                    values.Clear();

                    foreach (string val in tempList)
                    {
                        values.Add(val + "Call");
                        values.Add(val + "Raise");
                        values.Add(val + "Fold");
                    }

                    break;
            }

            switch (FourthType)
            {
                case ActionTypeEnum.Call:
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i] += "Call";
                    }

                    break;

                case ActionTypeEnum.Raise:
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i] += "Raise";
                    }

                    break;

                case ActionTypeEnum.Fold:
                    for (int i = 0; i < values.Count; i++)
                    {
                        values[i] += "Fold";
                    }

                    return result;

                case ActionTypeEnum.Any:
                    List<string> tempList = new List<string>(values);
                    values.Clear();

                    foreach (string val in tempList)
                    {
                        values.Add(val + "Call");
                        values.Add(val + "Raise");
                        values.Add(val + "Fold");
                    }
                    break;
            }

            foreach (string val in values)
            {
                switch (val)
                {
                    case "Fold":
                        result.Add(0);
                        break;
                    case "CheckFold":
                        result.Add(1);
                        break;
                    case "CheckCallFold":
                        result.Add(2);
                        break;
                    case "CheckCallCallFold":
                        result.Add(3);
                        break;
                    case "CheckCallCallCall":
                        result.Add(4);
                        break;
                    case "CheckCallCallRaise":
                        result.Add(5);
                        break;
                    case "CheckCallCall":
                        result.Add(6);
                        break;
                    case "CheckCallRaiseFold":
                        result.Add(7);
                        break;
                    case "CheckCallRaiseCall":
                        result.Add(8);
                        break;
                    case "CheckCallRaiseRaise":
                        result.Add(9);
                        break;
                    case "CheckCallRaise":
                        result.Add(10);
                        break;
                    case "CheckCall":
                        result.Add(11);
                        break;
                    case "CheckRaiseFold":
                        result.Add(12);
                        break;
                    case "CheckRaiseCallFold":
                        result.Add(13);
                        break;
                    case "CheckRaiseCallCall":
                        result.Add(14);
                        break;
                    case "CheckRaiseCallRaise":
                        result.Add(15);
                        break;
                    case "CheckRaiseCall":
                        result.Add(16);
                        break;
                    case "CheckRaiseRaiseFold":
                        result.Add(17);
                        break;
                    case "CheckRaiseRaiseCall":
                        result.Add(18);
                        break;
                    case "CheckRaiseRaiseRaise":
                        result.Add(19);
                        break;
                    case "CheckRaiseRaise":
                        result.Add(20);
                        break;
                    case "CheckRaise":
                        result.Add(21);
                        break;
                    case "Check":
                        result.Add(22);
                        break;
                    case "BetFold":
                        result.Add(23);
                        break;
                    case "BetCallFold":
                        result.Add(24);
                        break;
                    case "BetCallCallFold":
                        result.Add(25);
                        break;
                    case "BetCallCallCall":
                        result.Add(26);
                        break;
                    case "BetCallCallRaise":
                        result.Add(27);
                        break;
                    case "BetCallCall":
                        result.Add(28);
                        break;
                    case "BetCallRaiseFold":
                        result.Add(29);
                        break;
                    case "BetCallRaiseCall":
                        result.Add(30);
                        break;
                    case "BetCallRaiseRaise":
                        result.Add(31);
                        break;
                    case "BetCallRaise":
                        result.Add(32);
                        break;
                    case "BetCall":
                        result.Add(33);
                        break;
                    case "BetRaiseFold":
                        result.Add(34);
                        break;
                    case "BetRaiseCallFold":
                        result.Add(35);
                        break;
                    case "BetRaiseCallCall":
                        result.Add(36);
                        break;
                    case "BetRaiseCallRaise":
                        result.Add(37);
                        break;
                    case "BetRaiseCall":
                        result.Add(38);
                        break;
                    case "BetRaiseRaiseFold":
                        result.Add(39);
                        break;
                    case "BetRaiseRaiseCall":
                        result.Add(40);
                        break;
                    case "BetRaiseRaiseRaise":
                        result.Add(41);
                        break;
                    case "BetRaiseRaise":
                        result.Add(42);
                        break;
                    case "BetRaise":
                        result.Add(43);
                        break;
                    case "Bet":
                        result.Add(44);
                        break;
                    case "CallFold":
                        result.Add(45);
                        break;
                    case "CallCallFold":
                        result.Add(46);
                        break;
                    case "CallCallCallFold":
                        result.Add(47);
                        break;
                    case "CallCallCallCall":
                        result.Add(48);
                        break;
                    case "CallCallCallRaise":
                        result.Add(49);
                        break;
                    case "CallCallCall":
                        result.Add(50);
                        break;
                    case "CallCallRaiseFold":
                        result.Add(51);
                        break;
                    case "CallCallRaiseCall":
                        result.Add(52);
                        break;
                    case "CallCallRaiseRaise":
                        result.Add(53);
                        break;
                    case "CallCallRaise":
                        result.Add(54);
                        break;
                    case "CallCall":
                        result.Add(55);
                        break;
                    case "CallRaiseFold":
                        result.Add(56);
                        break;
                    case "CallRaiseCallFold":
                        result.Add(57);
                        break;
                    case "CallRaiseCallCall":
                        result.Add(58);
                        break;
                    case "CallRaiseCallRaise":
                        result.Add(59);
                        break;
                    case "CallRaiseCall":
                        result.Add(60);
                        break;
                    case "CallRaiseRaiseFold":
                        result.Add(61);
                        break;
                    case "CallRaiseRaiseCall":
                        result.Add(62);
                        break;
                    case "CallRaiseRaiseRaise":
                        result.Add(63);
                        break;
                    case "CallRaiseRaise":
                        result.Add(64);
                        break;
                    case "CallRaise":
                        result.Add(65);
                        break;
                    case "Call":
                        result.Add(66);
                        break;
                    case "RaiseFold":
                        result.Add(67);
                        break;
                    case "RaiseCallFold":
                        result.Add(68);
                        break;
                    case "RaiseCallCallFold":
                        result.Add(69);
                        break;
                    case "RaiseCallCallCall":
                        result.Add(70);
                        break;
                    case "RaiseCallCallRaise":
                        result.Add(71);
                        break;
                    case "RaiseCallCall":
                        result.Add(72);
                        break;
                    case "RaiseCallRaiseFold":
                        result.Add(73);
                        break;
                    case "RaiseCallRaiseCall":
                        result.Add(74);
                        break;
                    case "RaiseCallRaiseRaise":
                        result.Add(75);
                        break;
                    case "RaiseCallRaise":
                        result.Add(76);
                        break;
                    case "RaiseCall":
                        result.Add(77);
                        break;
                    case "RaiseRaiseFold":
                        result.Add(78);
                        break;
                    case "RaiseRaiseCallFold":
                        result.Add(79);
                        break;
                    case "RaiseRaiseCallCall":
                        result.Add(80);
                        break;
                    case "RaiseRaiseCallRaise":
                        result.Add(81);
                        break;
                    case "RaiseRaiseCall":
                        result.Add(82);
                        break;
                    case "RaiseRaiseRaiseFold":
                        result.Add(83);
                        break;
                    case "RaiseRaiseRaiseCall":
                        result.Add(84);
                        break;
                    case "RaiseRaiseRaiseRaise":
                        result.Add(85);
                        break;
                    case "RaiseRaiseRaise":
                        result.Add(86);
                        break;
                    case "RaiseRaise":
                        result.Add(87);
                        break;
                    case "Raise":
                        result.Add(88);
                        break;
                }
            }

            return result;
        }
    }
}