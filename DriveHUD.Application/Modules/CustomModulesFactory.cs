//-----------------------------------------------------------------------
// <copyright file="CustomModulesFactory.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model;
using System.Collections.Generic;

namespace DriveHUD.Application.Modules
{
    internal class CustomModulesFactory
    {
        public static IEnumerable<CustomModule> GetModules()
        {
            return new[]
            {
                new CustomModule(CustomModulesNames.PlayerXRay, CustomModulesNames.PlayerXRayAssembly),
                new CustomModule(CustomModulesNames.PMCatcher, CustomModulesNames.PMCatcherAssembly),
                new CustomModule(CustomModulesNames.PKCatcher, CustomModulesNames.PKCatcherAssembly)
            };
        }
    }
}