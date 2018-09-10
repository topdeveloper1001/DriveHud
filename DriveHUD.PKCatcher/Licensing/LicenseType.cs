﻿//-----------------------------------------------------------------------
// <copyright file="LicenseType.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Reflection;

namespace DriveHUD.PKCatcher.Licensing
{
    /// <summary>
    /// Types of license
    /// </summary>
    [Obfuscation(Exclude = true)]
    internal enum LicenseType : short
    {
        PKCTrial = 1,
        PKCNormal = 2
    } 
}