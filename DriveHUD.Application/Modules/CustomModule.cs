//-----------------------------------------------------------------------
// <copyright file="CustomModule.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Annotations;
using System;

namespace DriveHUD.Application.Modules
{
    /// <summary>
    /// Represents custom DH module
    /// </summary>
    internal class CustomModule
    {
        /// <summary>
        /// Creates the instance of <see cref="CustomModule"/> 
        /// </summary>
        /// <param name="moduleName">Name of module</param>
        /// <param name="moduleAssembly">Path to assembly of module</param>
        /// <exception cref="ArgumentNullException" />
        public CustomModule([NotNull] string moduleName, [NotNull] string moduleAssembly)
        {
            if (moduleName == null)
            {
                throw new ArgumentNullException(nameof(moduleName));
            }

            if (moduleAssembly == null)
            {
                throw new ArgumentNullException(nameof(moduleAssembly));
            }

            ModuleName = moduleName;
            ModuleAssembly = moduleAssembly;
        }

        /// <summary>
        /// Gets the path to module's assembly with entry point
        /// </summary>
        public string ModuleAssembly { get; private set; }

        /// <summary>
        /// Gets the name of module
        /// </summary>
        public string ModuleName { get; private set; }
    }
}