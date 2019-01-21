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
using System.Windows.Threading;

namespace DriveHUD.HUD
{
    /// <summary>
    /// This class is responsible for drawing HUD on table. It listens Win Events and manages size, position changes and also window close event
    /// </summary>
    internal static class HudPainter
    {
        private const int timerInterval = 50;
        private const Swp TopMostFlags = Swp.NOMOVE | Swp.NOSIZE | Swp.NOACTIVATE | Swp.NOOWNERZORDER | Swp.NOSENDCHANGING | Swp.NOREDRAW;

        private static Dictionary<IntPtr, HudWindowItem> windows = new Dictionary<IntPtr, HudWindowItem>();

        private static IntPtr moveHook;
        private static IntPtr closeHook;
        private static IntPtr createHook;
        private static IntPtr foregroundChangedHook;

        //Create delegate instances to prevent GC collecting.
        private static WinApi.WinEventDelegate moveCallback = WindowMoved;
        private static WinApi.WinEventDelegate closeCallback = WindowClosed;
        private static WinApi.WinEventDelegate createCallback = WindowMoved;
        private static WinApi.WinEventDelegate foregroundChangedCallback = ForegroundChanged;

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

            if (hudLayout.PokerSite == Entities.EnumPokerSites.Adda52)
            {
                foregroundChangedHook = WinApi.SetWinEventHook(WinApi.EVENT_SYSTEM_FOREGROUND, foregroundChangedCallback, processId);
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

                    var windowInteropHelper = CreateWindowInteropHelper(window, hudLayout, hwnd);

                    var windowItem = new HudWindowItem
                    {
                        Window = window,
                        Handle = windowHandle,
                        OverlayHandle = windowInteropHelper.EnsureHandle()
                    };

                    using (rwWindowsLock.Write())
                    {
                        if (windows.ContainsKey(hwnd))
                        {
                            return;
                        }

                        windows.Add(hwnd, windowItem);
                    }

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

                    PrepareWindow(windowItem, hudLayout, hwnd);

                    if (!WinApi.IsIconic(hwnd))
                    {
                        window.Refresh();
                    }

                    if (hudLayout.PokerSite == Entities.EnumPokerSites.PokerBaazi)
                    {
                        WinApi.GetWindowRect(hwnd, out RECT rct);
                        WinApi.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, rct.Width + 1, rct.Height + 1, Swp.NOMOVE | Swp.NOZORDER | Swp.NOACTIVATE);
                        WinApi.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, rct.Width, rct.Height, Swp.NOMOVE | Swp.NOZORDER | Swp.NOACTIVATE);
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

        private static WindowInteropHelper CreateWindowInteropHelper(HudWindow window, HudLayout hudLayout, IntPtr windowHandle)
        {
            if (hudLayout.PokerSite == Entities.EnumPokerSites.Adda52)
            {
                return new WindowInteropHelper(window);
            }

            return new WindowInteropHelper(window)
            {
                Owner = windowHandle
            };
        }

        private static void PrepareWindow(HudWindowItem windowItem, HudLayout hudLayout, IntPtr windowHandle)
        {
            if (hudLayout.PokerSite != Entities.EnumPokerSites.Adda52)
            {
                return;
            }

            var isHiddenWindow = false;

            var dispatcherTimer = new DispatcherTimer();

            var timerThrownException = false;
            var handle = windowItem.OverlayHandle;

            WinApi.SetWindowPos(handle, windowHandle, 0, 0, 0, 0, TopMostFlags);
            WinApi.SetWindowPos(windowHandle, handle, 0, 0, 0, 0, TopMostFlags);

            dispatcherTimer.Tick += (s, e) =>
            {
                try
                {
                    // handle minimize/restore 
                    if (!isHiddenWindow && WinApi.IsIconic(windowHandle))
                    {
                        windowItem.Window.Hide();
                        isHiddenWindow = true;
                    }
                    else if (isHiddenWindow && !WinApi.IsIconic(windowHandle))
                    {
                        windowItem.Window.Show();
                        isHiddenWindow = false;
                    }

                    var nextWindow = WinApi.GetWindow(handle, GetWindowType.GW_HWNDNEXT);

                    if (nextWindow != windowHandle)
                    {
                        WinApi.SetWindowPos(windowHandle, handle, 0, 0, 0, 0, TopMostFlags);
                    }

                    var activeWindow = WinApi.GetForegroundWindow();

                    if (activeWindow != handle)
                    {
                        if (activeWindow == windowHandle && !windowItem.IsTopMost)
                        {
                            WinApi.SetWindowPos(handle, Hwnd.TopMost, 0, 0, 0, 0, TopMostFlags);
                            windowItem.IsTopMost = true;
                        }
                        else if (activeWindow != windowHandle && windowItem.IsTopMost)
                        {
                            WinApi.SetWindowPos(handle, activeWindow, 0, 0, 0, 0, TopMostFlags);
                            WinApi.SetWindowPos(handle, Hwnd.NoTopMost, 0, 0, 0, 0, TopMostFlags);
                            windowItem.IsTopMost = false;
                        }
                    }
                    else if (!windowItem.IsTopMost)
                    {
                        WinApi.SetForegroundWindow(windowHandle);
                        WinApi.SetWindowPos(handle, Hwnd.TopMost, 0, 0, 0, 0, TopMostFlags);
                        windowItem.IsTopMost = true;
                    }
                }
                catch (Exception ex)
                {
                    if (!timerThrownException)
                    {
                        LogProvider.Log.Error(typeof(HudPainter), "Failed to prepare window", ex);
                        timerThrownException = true;
                    }
                }
            };

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(timerInterval);
            dispatcherTimer.Start();
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

        private static void ForegroundChanged(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            try
            {
                if (!windows.TryGetValue(hwnd, out HudWindowItem window) || !window.IsTopMost)
                {
                    return;
                }

                window.IsTopMost = true;
                WinApi.SetWindowPos(window.OverlayHandle, Hwnd.TopMost, 0, 0, 0, 0, TopMostFlags);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(typeof(HudPainter), "Failed to process foreground changed hook.", e);
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

                LogProvider.Log.Info(typeof(HudPainter), $"Window [{hwnd}] has been closed.");
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

            public IntPtr OverlayHandle { get; set; }

            public HudWindow Window { get; set; }

            public bool IsInitialized { get; set; }

            public bool IsTopMost { get; set; }
        }

        #endregion
    }
}