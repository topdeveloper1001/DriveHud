//-----------------------------------------------------------------------
// <copyright file="RegistryUtils.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Common.Utils
{
    public class RegistryUtils
    {
        public static string UninstallRegistryPath64Bit = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
        public static string UninstallRegistryPath32Bit = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public static bool UninstallRegistryContainsKeys(IEnumerable<string> registryKeys)
        {
            try
            {
                var uninstallKeys = new RegistryKey[]
                {
                    Registry.LocalMachine.OpenSubKey(UninstallRegistryPath64Bit),
                    Registry.LocalMachine.OpenSubKey(UninstallRegistryPath32Bit),
                };

                foreach (var uninstallKey in uninstallKeys)
                {
                    if (uninstallKey == null)
                    {
                        continue;
                    }

                    foreach (var registryKey in registryKeys)
                    {
                        try
                        {
                            var productKey = uninstallKey.OpenSubKey(registryKey);

                            if (productKey != null)
                            {
                                return true;
                            }
                        }
                        catch (Exception e)
                        {
                            LogProvider.Log.Error(typeof(RegistryUtils), $"Could not read registry key '{uninstallKey.Name}\\{registryKey}'", e);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(RegistryUtils), $"Could not read uninstall registry key", ex);
            }

            return false;
        }

        public static bool UninstallRegistryContainsDisplayNames(IEnumerable<string> displayNames)
        {
            try
            {
                var uninstallKeys = new RegistryKey[]
                {
                    Registry.LocalMachine.OpenSubKey(UninstallRegistryPath64Bit),
                    Registry.LocalMachine.OpenSubKey(UninstallRegistryPath32Bit),
                };

                foreach (var uninstallKey in uninstallKeys)
                {
                    if (uninstallKey == null)
                    {
                        continue;
                    }

                    foreach (var subKey in uninstallKey.GetSubKeyNames())
                    {
                        var productKey = uninstallKey.OpenSubKey(subKey);

                        var displayNameValue = productKey.GetValue("DisplayName");
                        var displayName = displayNameValue != null ? displayNameValue.ToString() : null;

                        if (!string.IsNullOrEmpty(displayName) && displayNames.Contains(displayName, StringComparer.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(RegistryUtils), $"Could not read uninstall registry display names", ex);
            }

            return false;
        }     
    }
}