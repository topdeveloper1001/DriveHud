//-----------------------------------------------------------------------
// <copyright file="PlayerXRayModule.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.CustomServices;
using DriveHUD.Common.Infrastructure.Modules;
using DriveHUD.PlayerXRay.Services;
using Microsoft.Practices.Unity;
using System;

namespace DriveHUD.PlayerXRay
{
    public class PlayerXRayModule : IDHModule
    {
        public void ConfigureContainer(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.RegisterType<IPlayerNotesService, PlayerXRayNoteService>(new ContainerControlledLifetimeManager());
            container.RegisterType<INoteProcessingService, NoteProcessingService>();
        }
    }
}