//-----------------------------------------------------------------------
// <copyright file="HotkeyHelper.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DriveHUD.Common.Utils
{
    public class HotkeyHelper : IDisposable
    {
        private const int WM_KEYDOWN = 0x0100;

        private WinApi.WinApi.LowLevelKeyboardProcDelegate lowLevelKeyboardProcDelegate;

        private IntPtr keyboardProcHook;

        private Dictionary<HotkeyCombination, Action> hotkeyActions = new Dictionary<HotkeyCombination, Action>();

        public void SetHotkeys()
        {
            try
            {
                lowLevelKeyboardProcDelegate = new WinApi.WinApi.LowLevelKeyboardProcDelegate(LowLevelKeyboardProc);

                using (var process = Process.GetCurrentProcess())
                {
                    using (var module = process.MainModule)
                    {
                        keyboardProcHook = WinApi.WinApi.SetWindowsHookEx(WinApi.WinApi.WH_KEYBOARD_LL,
                            lowLevelKeyboardProcDelegate, WinApi.WinApi.GetModuleHandle(module.ModuleName), 0);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Failed to set hotkeys.", e);
            }
        }

        public void UnsetHotkeys()
        {
            if (keyboardProcHook != IntPtr.Zero)
            {
                WinApi.WinApi.UnhookWindowsHookEx(keyboardProcHook);
            }

            hotkeyActions.Clear();
        }

        public void RegisterKeyAction(Keys key, HotkeyModifiers modifier, Action action)
        {
            var hotkeyCombination = new HotkeyCombination
            {
                Key = key,
                Modifier = modifier
            };

            hotkeyActions[hotkeyCombination] = action;
        }

        private IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                Keys keyPressed = (Keys)Marshal.ReadInt32(lParam);

                var modifier = HotkeyModifiers.None;

                if (GetCtrlPressed())
                {
                    modifier |= HotkeyModifiers.Ctrl;
                }

                if (GetAltPressed())
                {
                    modifier |= HotkeyModifiers.Alt;
                }

                if (GetAltPressed())
                {
                    modifier |= HotkeyModifiers.Shift;
                }

                var hotkeyCombination = new HotkeyCombination
                {
                    Key = keyPressed,
                    Modifier = modifier
                };

                if (hotkeyActions.TryGetValue(hotkeyCombination, out Action hotkeyAction))
                {                                        
                    hotkeyAction?.Invoke();
                }
            }

            return WinApi.WinApi.CallNextHookEx(
                keyboardProcHook, nCode, wParam, lParam);
        }

        private static bool GetCtrlPressed()
        {
            return GetKeyPressed(Keys.ControlKey);
        }

        private static bool GetAltPressed()
        {
            return GetKeyPressed(Keys.Menu);
        }

        private static bool GetShiftPressed()
        {
            return GetKeyPressed(Keys.ShiftKey);
        }

        private static bool GetKeyPressed(Keys key)
        {
            int state = WinApi.WinApi.GetKeyState(key);
            return (state > 1 || state < -1);
        }

        #region IDisposable implementation

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources here.                    
                    Disposing();
                }

                // Clear unmanaged resources here
                if (keyboardProcHook != IntPtr.Zero)
                {
                    WinApi.WinApi.UnhookWindowsHookEx(keyboardProcHook);
                }

                disposed = true;
            }
        }

        protected virtual void Disposing()
        {
        }

        ~HotkeyHelper()
        {
            Dispose(false);
        }

        #endregion      

        private class HotkeyCombination
        {
            public HotkeyModifiers Modifier { get; set; }

            public Keys Key { get; set; }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + Modifier.GetHashCode();
                    hashcode = (hashcode * 31) + Key.GetHashCode();
                    return hashcode;
                }
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as HotkeyCombination);
            }

            public bool Equals(HotkeyCombination hotkeyCombination)
            {
                return hotkeyCombination != null && hotkeyCombination.Key == Key &&
                    hotkeyCombination.Modifier == Modifier;
            }

            public override string ToString()
            {
                return $"Modifier: {Modifier}; Key: {Key}";
            }
        }
    }
}