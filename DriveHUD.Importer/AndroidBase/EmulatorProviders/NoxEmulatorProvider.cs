//-----------------------------------------------------------------------
// <copyright file="NoxEmulatorProvider.cs" company="Ace Poker Solutions">
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
    internal class NoxEmulatorProvider : VirtualBoxEmulator
    {
        // nox_adb.exe
        private static readonly byte[] location = new byte[] { 0x6E, 0x6F, 0x78, 0x5F, 0x61, 0x64, 0x62, 0x2E, 0x65, 0x78, 0x65 };

        protected override string EmulatorName => "NOX";

        protected override string ProcessName => "Nox";

        protected override string InstanceArgumentPrefix => "nox_";

        protected override string VbProcessName => "Nox";

        protected override string VbInstanceArgumentPrefix => "nox_";

        public override string GetAdbLocation()
        {
            return Encoding.ASCII.GetString(location);
        }
    }
}