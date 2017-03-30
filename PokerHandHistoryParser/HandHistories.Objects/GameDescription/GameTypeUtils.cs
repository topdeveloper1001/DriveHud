using DriveHUD.Common.Log;
using System;
using System.Linq;

namespace HandHistories.Objects.GameDescription
{
    public class GameTypeUtils
    {
        public static GameType ParseGameString(string gameString)
        {
            switch (gameString.ToLower())
            {
                case "nl":
                case "nlh":
                case "nl holdem":
                case "nl hold'em":
                case "no limit hold'em":
                case "no limit holdem":
                case "no limit":
                case "holdem no limit":
                    return GameType.NoLimitHoldem;
                case "pl":
                case "pot limit":
                case "pl holdem":
                case "pot limit holdem":
                case "pot-limit holdem":
                    return GameType.PotLimitHoldem;
                case "fl":
                case "fixed limit":
                case "fl holdem":
                case "fl hold'em":
                case "fixed holdem":
                case "fixed hold'em":
                    return GameType.FixedLimitHoldem;
                case "pl omaha":
                case "plo":
                case "omaha":
                case "pot limit omaha":
                case "pot-limit omaha":
                case "omaha pot limit":
                    return GameType.PotLimitOmaha;
                case "pl omaha h/l":
                case "plo hi-lo":
                    return GameType.PotLimitOmahaHiLo;
                case "omaha hi-lo no limit":
                    return GameType.NoLimitOmahaHiLo;
                default:
                    string match = Enum.GetNames(typeof(GameType)).FirstOrDefault(g => g.ToLower().Equals(gameString.ToLower()));
                    return match == null ? GameType.Unknown : (GameType)Enum.Parse(typeof(GameType), match, true);
            }
        }

        public static string GetGameName(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.NoLimitHoldem:
                    return "NL Holdem";
                case GameType.FixedLimitHoldem:
                    return "FL Holdem";
                case GameType.PotLimitOmaha:
                    return "PLO";
                case GameType.PotLimitOmahaHiLo:
                    return "PLO Hi-Lo";
                case GameType.PotLimitHoldem:
                    return "Pot Limit Holdem";
                case GameType.Any:
                    return "Any";
                case GameType.CapNoLimitHoldem:
                    return "Cap NL Holdem";
                case GameType.CapPotLimitOmaha:
                    return "Cap Pot Limit Omaha";
                case GameType.Unknown:
                    return "Unknown";
                case GameType.FixedLimitOmahaHiLo:
                    return "FL Omaha Hi-Lo";
                case GameType.NoLimitOmahaHiLo:
                    return "No Limit Omaha Hi-Lo";
                case GameType.NoLimitOmaha:
                    return "No Limit Omaha";
                case GameType.FiveCardPotLimitOmahaHiLo:
                    return "Pot Limit Five Card Omaha Hi-Lo";
                case GameType.FiveCardPotLimitOmaha:
                    return "Pot Limit Five Card Omaha";
                case GameType.FixedLimitOmaha:
                    return "Fixed Limit Omaha";
                case GameType.SpreadLimitHoldem:
                    return "Spread Limit Holdem";
                default:
                    LogProvider.Log.Warn("GetGameName: Not implemented for " + gameType);
                    return gameType.ToString();
            }
        }

        public static string GetShortName(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.NoLimitHoldem:
                    return "NLH";
                case GameType.FixedLimitHoldem:
                    return "FLH";
                case GameType.PotLimitOmaha:
                    return "PLO";
                case GameType.PotLimitOmahaHiLo:
                    return "PLOHiLo";
                case GameType.PotLimitHoldem:
                    return "PLH";
                case GameType.Any:
                    return "Any";
                case GameType.CapNoLimitHoldem:
                    return "CapNLH";
                case GameType.CapPotLimitOmaha:
                    return "CapPLO";
                case GameType.Unknown:
                    return "Unknown";
                case GameType.FixedLimitOmahaHiLo:
                    return "FLOmahaHiLo";
                case GameType.NoLimitOmahaHiLo:
                    return "NLOmahaHiLo";
                case GameType.NoLimitOmaha:
                    return "NLOmaha";
                case GameType.FiveCardPotLimitOmahaHiLo:
                    return "5Card-PLOHiLo";
                case GameType.FiveCardPotLimitOmaha:
                    return "5Card-PLO";
                case GameType.FixedLimitOmaha:
                    return "FLOmaha";
                case GameType.SpreadLimitHoldem:
                    return "SLHoldem";
                default:
                    LogProvider.Log.Warn("GetGameName: Not implemented for " + gameType);
                    return gameType.ToString();
            }
        }

        public static string GetDisplayName(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.NoLimitHoldem:
                    return "No Limit Holdem";
                case GameType.FixedLimitHoldem:
                    return "Fixed Limit Holdem";
                case GameType.PotLimitOmaha:
                    return "Pot Limit Omaha";
                case GameType.PotLimitOmahaHiLo:
                    return "Pot Limit Omaha Hi-Lo";
                case GameType.FixedLimitOmahaHiLo:
                    return "Fixed Limit Omaha Hi-Lo";
                case GameType.PotLimitHoldem:
                    return "Pot Limit Holdem";
                case GameType.Any:
                    return "Any";
                case GameType.CapNoLimitHoldem:
                    return "Cap NL Holdem";
                case GameType.CapPotLimitOmaha:
                    return "Cap Pot Limit Omaha";
                case GameType.Unknown:
                    return "Unknown";
                default:
                    return gameType.ToString();
            }
        }

        public static bool CompareGameType(GameType gameTypeA, GameType gameTypeB)
        {
            if (gameTypeA == gameTypeB)
            {
                return true;
            }

            var simpleGameTypeA = ConvertGameTypeToSimple(gameTypeA);
            var simpleGameTypeB = ConvertGameTypeToSimple(gameTypeB);

            return simpleGameTypeA == simpleGameTypeB;
        }

        private static SimpleGameType ConvertGameTypeToSimple(GameType gameType)
        {
            switch (gameType)
            {
                case GameType.CapPotLimitOmaha:
                case GameType.FiveCardPotLimitOmaha:
                case GameType.FixedLimitOmaha:
                case GameType.NoLimitOmaha:
                case GameType.PotLimitOmaha:
                    return SimpleGameType.Omaha;
                case GameType.FiveCardPotLimitOmahaHiLo:
                case GameType.FixedLimitOmahaHiLo:
                case GameType.NoLimitOmahaHiLo:
                case GameType.PotLimitOmahaHiLo:
                    return SimpleGameType.OmahaHiLo;
                default:
                    return SimpleGameType.Holdem;
            }
        }

        private enum SimpleGameType
        {
            Holdem,
            Omaha,
            OmahaHiLo
        }
    }
}