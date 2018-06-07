//-----------------------------------------------------------------------
// <copyright file="WindowPositionsService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.Services
{
    internal class WindowPositionsService
    {
        public static void SetPosition(RadWindow window, WindowPositionsInfo positionInfo)
        {
            if (positionInfo == null)
            {
                throw new ArgumentNullException(nameof(positionInfo));
            }

            if (!TryGetWindowPosition(positionInfo, out Rectangle position))
            {
                return;
            }

            window.WindowStartupLocation = WindowStartupLocation.Manual;

            window.Left = position.X;
            window.Top = position.Y;

            if (position.Width > 0)
            {
                window.Width = position.Width;
            }

            if (position.Height > 0)
            {
                window.Height = position.Height;
            }

            if (positionInfo.DisplaySettings != null)
            {
                window.WindowState = positionInfo.DisplaySettings.Maximized ?
                    WindowState.Maximized :
                    WindowState.Normal;
            }
        }

        public static void SetPosition(Window window, WindowPositionsInfo positionInfo)
        {
            if (positionInfo == null)
            {
                throw new ArgumentNullException(nameof(positionInfo));
            }

            if (!TryGetWindowPosition(positionInfo, out Rectangle position))
            {
                return;
            }

            window.WindowStartupLocation = WindowStartupLocation.Manual;

            window.Left = position.X;
            window.Top = position.Y;

            if (position.Width > 0)
            {
                window.Width = position.Width;
            }

            if (position.Height > 0)
            {
                window.Height = position.Height;
            }

            if (positionInfo.DisplaySettings != null)
            {
                window.WindowState = positionInfo.DisplaySettings.Maximized ?
                    WindowState.Maximized :
                    WindowState.Normal;
            }
        }

        private static bool TryGetWindowPosition(WindowPositionsInfo positionsInfo, out Rectangle position)
        {
            double left = 0, top = 0, width = 0, height = 0;

            // use settings
            if (positionsInfo.DisplaySettings != null)
            {
                // get screen
                var screen = Screen.AllScreens
                    .FirstOrDefault(x => positionsInfo.DisplaySettings.Left >= x.Bounds.Left &&
                        positionsInfo.DisplaySettings.Left <= x.Bounds.Right &&
                        positionsInfo.DisplaySettings.Top >= x.Bounds.Top &&
                        positionsInfo.DisplaySettings.Top <= x.Bounds.Bottom);

                if (screen != null)
                {
                    if (positionsInfo.StartupLocation == WindowStartupLocation.CenterScreen)
                    {
                        if (positionsInfo.Width <= screen.Bounds.Width)
                        {
                            left = screen.Bounds.Left + (screen.Bounds.Width - positionsInfo.Width) / 2;
                            width = positionsInfo.Width;
                        }
                        else
                        {
                            left = screen.Bounds.Left;
                            width = screen.Bounds.Width;
                        }

                        if (positionsInfo.Height <= screen.Bounds.Height)
                        {
                            top = screen.Bounds.Top + (screen.Bounds.Height - positionsInfo.Height) / 2;
                            height = positionsInfo.Height;
                        }
                        else
                        {
                            top = screen.Bounds.Top;
                            height = screen.Bounds.Height;
                        }
                    }
                    else
                    {
                        left = positionsInfo.DisplaySettings.Left;
                        top = positionsInfo.DisplaySettings.Top;
                        width = positionsInfo.DisplaySettings.Width;
                        height = positionsInfo.DisplaySettings.Height;

                        if (left + width > screen.WorkingArea.Right)
                        {
                            var shiftLeft = left + width - screen.WorkingArea.Right;

                            if (shiftLeft > left - screen.WorkingArea.Left)
                            {
                                shiftLeft = shiftLeft - left + screen.WorkingArea.Left;
                                left = screen.WorkingArea.Left;

                                if (width - shiftLeft > screen.WorkingArea.Width)
                                {
                                    width = screen.WorkingArea.Width;
                                }
                                else
                                {
                                    width = width - shiftLeft;
                                }
                            }
                            else
                            {
                                left = left - shiftLeft;
                            }
                        }

                        if (top < 0)
                        {
                            top = 0;
                        }

                        if (top + height > screen.WorkingArea.Bottom)
                        {
                            var shiftTop = top + height - screen.WorkingArea.Bottom;

                            if (shiftTop > top - screen.WorkingArea.Top)
                            {
                                shiftTop = shiftTop - top + screen.WorkingArea.Top;
                                top = screen.WorkingArea.Top;

                                if (height - shiftTop > screen.WorkingArea.Height)
                                {
                                    height = screen.WorkingArea.Height;
                                }
                                else
                                {
                                    height = height - shiftTop;
                                }
                            }
                            else
                            {
                                top = top - shiftTop;
                            }
                        }
                    }

                    position = new Rectangle((int)left, (int)top, (int)width, (int)height);
                    return true;
                }
            }

            var primaryScreen = Screen.AllScreens.FirstOrDefault(x => x.Primary);

            if (primaryScreen == null)
            {
                position = new Rectangle();
                return false;
            }

            if (positionsInfo.Width <= primaryScreen.WorkingArea.Width)
            {
                left = primaryScreen.WorkingArea.Left + (primaryScreen.WorkingArea.Width - positionsInfo.Width) / 2;
                width = positionsInfo.Width;
            }
            else
            {
                left = primaryScreen.WorkingArea.Left;
                width = primaryScreen.WorkingArea.Width;
            }

            if (positionsInfo.Height <= primaryScreen.WorkingArea.Height)
            {
                top = primaryScreen.WorkingArea.Top + (primaryScreen.WorkingArea.Height - positionsInfo.Height) / 2;
                height = positionsInfo.Height;
            }
            else
            {
                top = primaryScreen.WorkingArea.Top;
                height = primaryScreen.WorkingArea.Height;
            }

            position = new Rectangle((int)left, (int)top, (int)width, (int)height);
            return true;
        }
    }
}