using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Enums
{
    public enum GeneralGameTypeEnum
    {
        Holdem, Omaha, OmahaHiLo
    }

    #region GeneralGameTypeEnum  Extensions

    public static class GeneralGameTypeEnumExtension
    {
        public static GeneralGameTypeEnum ParseGameType(this GeneralGameTypeEnum value, HandHistories.Objects.GameDescription.GameType gameType)
        {
            switch (gameType)
            {
                case HandHistories.Objects.GameDescription.GameType.CapPotLimitOmaha:
                case HandHistories.Objects.GameDescription.GameType.PotLimitOmaha:
                case HandHistories.Objects.GameDescription.GameType.FiveCardPotLimitOmaha:
                case HandHistories.Objects.GameDescription.GameType.NoLimitOmaha:
                case HandHistories.Objects.GameDescription.GameType.FixedLimitOmaha:
                    return GeneralGameTypeEnum.Omaha;

                case HandHistories.Objects.GameDescription.GameType.PotLimitOmahaHiLo:
                case HandHistories.Objects.GameDescription.GameType.FiveCardPotLimitOmahaHiLo:
                case HandHistories.Objects.GameDescription.GameType.NoLimitOmahaHiLo:
                case HandHistories.Objects.GameDescription.GameType.FixedLimitOmahaHiLo:
                    return GeneralGameTypeEnum.OmahaHiLo;

                case HandHistories.Objects.GameDescription.GameType.CapNoLimitHoldem:
                case HandHistories.Objects.GameDescription.GameType.PotLimitHoldem:
                case HandHistories.Objects.GameDescription.GameType.NoLimitHoldem:
                case HandHistories.Objects.GameDescription.GameType.FixedLimitHoldem:
                case HandHistories.Objects.GameDescription.GameType.Unknown:
                case HandHistories.Objects.GameDescription.GameType.SpreadLimitHoldem:
                case HandHistories.Objects.GameDescription.GameType.Any:
                default:
                    return GeneralGameTypeEnum.Holdem;
            }
        }
    }

    #endregion
}
