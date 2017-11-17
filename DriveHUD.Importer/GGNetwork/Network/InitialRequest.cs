//-----------------------------------------------------------------------
// <copyright file="InitialRequest.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.GGNetwork.Network
{
    public class InitialRequest
    {     
        public string Token { get; set; }

        public string BrandId { get; set; }

        public string ProtocolVersion { get; set; }

        public string HardwareSerialNumber { get; set; }

        public string MacAddress { get; set; }

        public int UserDeviceType { get; set; }

        public int UserOsType { get; set; }
    }
}