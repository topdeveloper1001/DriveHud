using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal partial class TempConfig
    {
        internal static TempConfig instance;
        internal static void Init()
        {
            SkipCorrections = SkipCorrections_Default;
            CorrectionAOpp = CorrectionAOpp_Default;
            CorrectionBOpp = CorrectionBOpp_Default;
            CorrectionAHero = CorrectionAHero_Default;
            CorrectionBHero = CorrectionBHero_Default;

            EnableMultiwayMultiplier = EnableMultiwayMultiplier_Default;
            MultiwayMultiplier_Inf_Last = MultiwayMultiplier_Inf_Last_Default;
            MultiwayMultiplier_Inf_NotLast = MultiwayMultiplier_Inf_NotLast_Default;
            MultiwayMultiplier_Sup_Last = MultiwayMultiplier_Sup_Last_Default;
            MultiwayMultiplier_Sup_NotLast = MultiwayMultiplier_Sup_NotLast_Default;

        }

        internal static bool SkipCorrections;
        internal static double CorrectionAOpp;
        internal static double CorrectionBOpp;
        internal static double CorrectionAHero;
        internal static double CorrectionBHero;

        internal static bool EnableMultiwayMultiplier;
        internal static double MultiwayMultiplier_Inf_Last;
        internal static double MultiwayMultiplier_Inf_NotLast;
        internal static double MultiwayMultiplier_Sup_Last;
        internal static double MultiwayMultiplier_Sup_NotLast;




        internal static bool SkipCorrections_Default = false;
        internal static double CorrectionAOpp_Default = 0;
        internal static double CorrectionBOpp_Default = 1.3;
        internal static double CorrectionAHero_Default = 0;
        internal static double CorrectionBHero_Default = 1.06;

        internal static bool EnableMultiwayMultiplier_Default = true;
        internal static double MultiwayMultiplier_Inf_Last_Default = 0.8;
        internal static double MultiwayMultiplier_Inf_NotLast_Default = 0.75;
        internal static double MultiwayMultiplier_Sup_Last_Default = 0.70;
        internal static double MultiwayMultiplier_Sup_NotLast_Default = 0.65;


        internal String originalCards;
        internal static String TempCards;
    }
}
