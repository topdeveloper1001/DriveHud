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

using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.Views;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using ManagedWinapi.Windows;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;

namespace DriveHUD.HUD
{
    /// <summary>
    /// This class is responsible for drawing om Bovada table. It listens Win Events and manages size, position changes and also window close event
    /// </summary>
    internal static class HudPainter
    {
        private static Dictionary<IntPtr, HudWindowItem> windows = new Dictionary<IntPtr, HudWindowItem>();

        #region PInvoke and Win Events      

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("User32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        private static IntPtr SetWinEventHook(uint eventId, WinEventDelegate callback, uint idProcess)
        {
            return SetWinEventHook(eventId, eventId, IntPtr.Zero, callback, idProcess, 0, WINEVENT_OUTOFCONTEXT);
        }

        public static HudLayout layout;

        public static string GetText(IntPtr hWnd)
        {
            // Allocate correct string length first
            int length = WinApi.GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            WinApi.GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
            {
                WinApi.EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);
            }

            return handles;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        static List<IntPtr> GetChildWindows(IntPtr hParent, int maxCount = 100)
        {
            List<IntPtr> result = new List<IntPtr>();
            int ct = 0;
            IntPtr prevChild = IntPtr.Zero;
            IntPtr currChild = IntPtr.Zero;
            while (ct < maxCount)
            {
                currChild = WinApi.FindWindowEx(hParent, prevChild, null, null);
                if (currChild == IntPtr.Zero) break;
                result.Add(currChild);
                prevChild = currChild;
                ++ct;
            }
            return result;
        }

        private static IntPtr moveHook;
        private static IntPtr closeHook;
        private static IntPtr createHook;

        private const uint GW_OWNER = 4;
        private const uint EVENT_SYSTEM_MOVESIZEEND = 0x800B;
        private const uint EVENT_OBJECT_CREATE = 0x8000;
        private const uint EVENT_OBJECT_DESTROY = 0x8001;
        private const uint EVENT_OBJECT_NAMECHANGE = 0x800C;
        private const uint WINEVENT_OUTOFCONTEXT = 0;

        //Create delegate instances to prevent GC collecting.
        private static WinEventDelegate moveCallback = WindowMoved;
        private static WinEventDelegate closeCallback = WindowClosed;
        private static WinEventDelegate createCallback = WindowMoved;

        #endregion

        public static bool IsStarted { get; set; }

        public static void UpdateHud(HudLayout hudLayout)
        {
            LogProvider.Log.Info(string.Format("Memory used before updating HUD: {0:N0}", GC.GetTotalMemory(false)));

            if (hudLayout == null || hudLayout.TableHud == null || hudLayout.TableHud.TableLayout == null)
            {
                return;
            }

            var hwnd = new IntPtr(hudLayout.WindowId);

            if (!IsWindow(hwnd))
            {
                return;
            }

            if (windows.ContainsKey(hwnd))
            {
                windows[hwnd].Window.Init(hudLayout);
                windows[hwnd].Window.Update();

                LogProvider.Log.Info(string.Format("Memory after updating HUD: {0:N0}", GC.GetTotalMemory(false)));

                return;
            }

            uint processId = 0;

            GetWindowThreadProcessId(hwnd, out processId);

            if (processId == 0)
            {
                return;
            }

            moveHook = SetWinEventHook(EVENT_SYSTEM_MOVESIZEEND, moveCallback, processId);
            closeHook = SetWinEventHook(EVENT_OBJECT_DESTROY, closeCallback, processId);
            createHook = SetWinEventHook(EVENT_OBJECT_NAMECHANGE, createCallback, processId);

            var hudPanelService = ServiceLocator.Current.GetInstance<IHudPanelService>(hudLayout.TableHud.TableLayout.Site.ToString());

            var window = new HudWindow();
            var windowHandle = hudPanelService.GetWindowHandle(hwnd);

            var windowItem = new HudWindowItem
            {
                Window = window,
                Handle = windowHandle
            };

            windows.Add(hwnd, windowItem);

            var windowInteropHelper = new WindowInteropHelper(window)
            {
                Owner = windowHandle
            };

            window.Init(hudLayout);

            window.Show();

            RECT rect;

            GetWindowRect(windowHandle, out rect);

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

            window.Update();

            LogProvider.Log.Info(string.Format("Memory after updating HUD: {0:N0}", GC.GetTotalMemory(false)));
        }

        public static void ReleaseHook()
        {
            IsStarted = false;

            UnhookWinEvent(moveHook);
            UnhookWinEvent(closeHook);
            UnhookWinEvent(createHook);

            foreach (var windowItem in windows.Values)
            {
                windowItem.Window.Close();
            }

            windows.Clear();
        }

        #region Infrastructure

        private static void WindowMoved(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // filter out non-HWND name changes... (eg. items within a listbox)
            if (idObject != 0 || idChild != 0)
            {
                return;
            }

            UpdateWindowOverlay(hwnd);
        }

        private static void WindowClosed(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // filter out non-HWND name changes... (eg. items within a listbox)
            if (idObject != 0 || idChild != 0)
            {
                return;
            }

            if (!windows.ContainsKey(hwnd))
                return;

            var window = windows[hwnd];
            windows.Remove(hwnd);
            window.Window.Close();

            LogProvider.Log.Info(string.Format("Memory used before collection: {0:N0}", GC.GetTotalMemory(false)));

            GC.Collect();

            LogProvider.Log.Info(string.Format("Memory used after collection: {0:N0}", GC.GetTotalMemory(false)));
        }

        private static void UpdateWindowOverlay(IntPtr handle)
        {
            if (!windows.ContainsKey(handle))
            {
                return;
            }

            var windowItem = windows[handle];

            RECT rect;

            GetWindowRect(windowItem.Handle, out rect);

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

            windowItem.Window.Update();
        }

        private class HudWindowItem
        {
            public IntPtr Handle { get; set; }

            public HudWindow Window { get; set; }
        }

        #endregion
    }
}