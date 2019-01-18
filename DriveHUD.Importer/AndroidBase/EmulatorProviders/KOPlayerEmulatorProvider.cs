//-----------------------------------------------------------------------
// <copyright file="KOPlayerEmulatorProvider.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.AndroidBase.EmulatorProviders
{
    internal class KOPlayerEmulatorProvider : VirtualBoxEmulator
    {
        protected override string EmulatorName => "KOPlayer";

        protected override string ProcessName => "KOPLAYER";

        protected override string VbProcessName => "VBoxHeadless";

        protected override string InstanceArgumentPrefix => "-n ";

        protected override string VbInstanceArgumentPrefix => "KOPLAYER_";

        protected override int? VbEmptyInstanceNumber => 0;
    }
}