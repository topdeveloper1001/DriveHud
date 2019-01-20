//-----------------------------------------------------------------------
// <copyright file="MemuEmulatorProvider.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Text;

namespace DriveHUD.Importers.AndroidBase.EmulatorProviders
{
    internal class MemuEmulatorProvider : VirtualBoxEmulator
    {
        // adb.exe
        private static readonly byte[] location = new byte[] { 0x61, 0x64, 0x62, 0x2E, 0x65, 0x78, 0x65 };

        protected override string EmulatorName => "MEmu";

        protected override string ProcessName => "Memu";

        protected override string InstanceArgumentPrefix => "memu_";

        protected override string VbProcessName => "Memu";

        protected override string VbInstanceArgumentPrefix => "memu_";

        public override string GetAdbLocation()
        {
            return Encoding.ASCII.GetString(location);
        }
    }
}