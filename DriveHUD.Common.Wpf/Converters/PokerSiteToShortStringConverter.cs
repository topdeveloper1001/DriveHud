using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

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
