//-----------------------------------------------------------------------
// <copyright file="HudPainter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.Views;
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Interop;

namespace DriveHUD.HUD
{
    /// <summary>
    /// This class is responsible for drawing HUD on table. It listens Win Events and manages size, position changes and also window close event
    /// </summary>
    internal static class HudPainter
    {
        private static Dictionary<IntPtr, HudWindowItem> windows = new Dictionary<IntPtr, HudWindowItem>();

        private static IntPtr moveHook;
        private static IntPtr closeHook;
        private static IntPtr createHook;
        private static IntPtr createCreateHook;

        //Create delegate instances to prevent GC collecting.
        private static WinApi.WinEventDelegate moveCallback = WindowMoved;
        private static WinApi.WinEventDelegate closeCallback = WindowClosed;
        private static WinApi.WinEventDelegate createCallback = WindowMoved;
        private static WinApi.WinEventDelegate createWindowCallback = WindowCreated;

        private static readonly ReaderWriterLockSlim rwWindowsLock = new ReaderWriterLockSlim();

        public static void UpdateHud(HudLayout hudLayout)
        {
            if (hudLayout == null)
            {
                return;
            }

            var hwnd = new IntPtr(hudLayout.WindowId);

            if (!WinApi.IsWindow(hwnd))
            {
                LogProvider.Log.Info($"Window {hwnd} doesn't exist.");
                return;
            }

            using (rwWindowsLock.Read())
            {
                if (windows.ContainsKey(hwnd))
                {
                    if (!windows[hwnd].IsInitialized)
                    {
                        return;
                    }

                    windows[hwnd].Window.Dispatcher.Invoke(() =>
                    {
                        windows[hwnd].Window.Initialize(hudLayout, hwnd);

                        if (!WinApi.IsIconic(hwnd))
                        {
                            windows[hwnd].Window.Refresh();
                        }
                    });

                    return;
                }
            }

            uint processId = 0;

            WinApi.GetWindowThreadProcessId(hwnd, out processId);

            if (processId == 0)
            {
                return;
            }

            moveHook = WinApi.SetWinEventHook(WinApi.EVENT_SYSTEM_MOVESIZEEND, moveCallback, processId);
            closeHook = WinApi.SetWinEventHook(WinApi.EVENT_OBJECT_DESTROY, closeCallback, processId);
            createHook = WinApi.SetWinEventHook(WinApi.EVENT_OBJECT_NAMECHANGE, createCallback, processId);

            // dirty workaround for Adda52 issues
            if (hudLayout.PokerSite == Entities.EnumPokerSites.Adda52)
            {
                createCreateHook = WinApi.SetWinEventHook(WinApi.EVENT_OBJECT_CREATE, createWindowCallback, processId);
            }

            CreateHudWindow(hwnd, hudLayout);
        }

        public static void CloseHudWindow(int handle)
        {
            if (handle == 0)
            {
                return;
            }

            var hwnd = new IntPtr(handle);

            if (!windows.ContainsKey(hwnd))
            {
                return;
            }

            var window = windows[hwnd];
            window.Window.Dispatcher.Invoke(() => window.Window.Close());
            windows.Remove(hwnd);
        }

        private static void CreateHudWindow(IntPtr hwnd, HudLayout hudLayout)
        {
            var thread = new Thread(() =>
            {
                try
                {
                    var hudPanelService = ServiceLocator.Current.GetInstance<IHudPanelService>(hudLayout.PokerSite.ToString());

                    var window = new HudWindow();
                    var windowHandle = hudPanelService.GetWindowHandle(hwnd);

                    if (!WinApi.IsWindow(windowHandle))
                    {
                        LogProvider.Log.Info(typeof(HudPainter), $"Window {windowHandle} doesn't exist. HUD window can't be created.");
                        return;
                    }

                    var windowItem = new HudWindowItem
                    {
                        Window = window,
                        Handle = windowHandle
                    };

                    using (rwWindowsLock.Write())
                    {
                        if (windows.ContainsKey(hwnd))
                        {
                            return;
                        }

                        windows.Add(hwnd, windowItem);
                    }

                    var windowInteropHelper = new WindowInteropHelper(window)
                    {
                        Owner = windowHandle
                    };

                    window.Closed += (s, e) => window.Dispatcher.InvokeShutdown();

                    window.Initialize(hudLayout, hwnd);

                    window.Show();

                    WinApi.GetWindowRect(windowHandle, out RECT rect);

                    SizeF dpi = Utils.GetCurrentDpi();

                    SizeF scale = new SizeF()
                    {
                        Width = 96f / dpi.Width,
                        Height = 96f / dpi.Height
                    };

                    window.Top = rect.Top * scale.Height;
                    window.Left = rect.Left * scale.Width;
                    window.Height = rect.Height * scale.Height;
                    window.Width = rect.Width * scale.Width;

                    windowItem.IsInitialized = true;

                    if (!WinApi.IsIconic(hwnd))
                    {
                        window.Refresh();
                    }

                    System.Windows.Threading.Dispatcher.Run();
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(typeof(HudPainter), $"Failed to create HUD window for {hwnd}", e);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        #region Infrastructure

        private static void WindowMoved(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0)
            {
                return;
            }

            UpdateWindowOverlay(hwnd);
        }

        private static void WindowCreated(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0)
            {
                return;
            }

            try
            {
                if (!WinApi.IsWindow(hwnd))
                {
                    return;
                }

                WinApi.ShowWindow(hwnd, ShowWindowCommands.Minimize);
                Thread.Sleep(150);
                WinApi.ShowWindow(hwnd, ShowWindowCommands.Restore);
            }
            catch
            {
            }
        }

        private static void WindowClosed(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0)
            {
                return;
            }

            if (!windows.ContainsKey(hwnd))
            {
                return;
            }

            try
            {
                var window = windows[hwnd];
                windows.Remove(hwnd);
                window.Window.Dispatcher.Invoke(() => window.Window.Close());
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(typeof(HudPainter), $"Error occurred during the attempt to close window. [{hwnd}]", e);
            }
        }

        private static void UpdateWindowOverlay(IntPtr handle)
        {
            if (!windows.ContainsKey(handle))
            {
                return;
            }

            if (WinApi.IsIconic(handle))
            {
                return;
            }

            HudWindowItem windowItem = null;

            windowItem = windows[handle];

            if (windowItem == null || !windowItem.IsInitialized)
            {
                return;
            }

            windowItem.Window.Dispatcher.Invoke(() =>
            {
                WinApi.GetWindowRect(windowItem.Handle, out RECT rect);

                SizeF dpi = Utils.GetCurrentDpi();

                SizeF scale = new SizeF()
                {
                    Width = 96f / dpi.Width,
                    Height = 96f / dpi.Height
                };

                windowItem.Window.Top = rect.Top * scale.Height;
                windowItem.Window.Left = rect.Left * scale.Width;
                windowItem.Window.Height = rect.Height * scale.Height;
                windowItem.Window.Width = rect.Width * scale.Width;

                windowItem.Window.Refresh();
            });
        }

        private class HudWindowItem
        {
            public IntPtr Handle { get; set; }

            public HudWindow Window { get; set; }

            public bool IsInitialized { get; set; }
        }

        #endregion       
    }
}