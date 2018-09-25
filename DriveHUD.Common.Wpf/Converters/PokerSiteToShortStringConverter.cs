using DriveHUD.Entities;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DriveHUD.Common.Wpf.Converters
{
    [ValueConversion(typeof(EnumPokerSites), typeof(string))]
    public class PokerSiteToShortStringConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return EnumPokerSites.Unknown;
            }

            EnumPokerSites site;

            if (Enum.TryParse(value.ToString(), out site))
            {
                switch (site)
                {
                    case EnumPokerSites.TigerGaming:
                        return "TG";
                    case EnumPokerSites.SportsBetting:
                        return "SB";
                    case EnumPokerSites.BetOnline:
                        return "BOL";
                    case EnumPokerSites.Poker888:
                        return "888";
                    case EnumPokerSites.WinningPokerNetwork:
                        return "WPN";
                    case EnumPokerSites.AmericasCardroom:
                        return "ACR";
                    case EnumPokerSites.BlackChipPoker:
                        return "BCP";
                    case EnumPokerSites.TruePoker:
                        return "TP";
                    case EnumPokerSites.YaPoker:
                        return "YP";
                    case EnumPokerSites.PartyPoker:
                        return "PP";
                    case EnumPokerSites.Horizon:
                        return "REV";
                    case EnumPokerSites.Adda52:
                        return "ADDA";
                    case EnumPokerSites.SpartanPoker:
                        return "TSP";
                    default:
                        break;
                }
            }

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
