﻿//-----------------------------------------------------------------------
// <copyright file="HudReceiver.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Log;
using ProtoBuf;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.HUD.Services
{
    internal class HudReceiver : IHudReceiver
    {
        private const int delay = 2000;
        private AnonymousPipeClientStream pipeClient;
        private bool isInitialized;

        public void Initialize(string clientHandle)
        {
            try
            {
                pipeClient = new AnonymousPipeClientStream(PipeDirection.In, clientHandle);
                isInitialized = true;

                LogProvider.Log.Info("HUD service has been initialized");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "HUD service cannot be initialized", e);
            }
        }

        public void Start()
        {
            if (!isInitialized)
            {
                LogProvider.Log.Error("HUD service hasn't been initialized");
                return;
            }

            Task.Run(() => ReadData());
        }

        private void ReadData()
        {
            LogProvider.Log.Info("Ready to read data");

            try
            {
                while (true)
                {
                    if (!pipeClient.IsConnected)
                    {
                        Task.Delay(delay).Wait();
                        continue;
                    }

                    try
                    {
                        var buffer = new byte[16384];
                        byte[] data;

                        using (var ms = new MemoryStream())
                        {
                            using (var reader = new BinaryReader(pipeClient, Encoding.UTF8, true))
                            {
                                while (true)
                                {
                                    var read = pipeClient.Read(buffer, 0, buffer.Length);

                                    ms.Write(buffer, 0, read);

                                    if (read < buffer.Length)
                                    {
                                        break;
                                    }
                                }

                                data = ms.ToArray();
                            }
                        }

                        if (data.Length > 0)
                        {
                            HudLayout hudLayout;

                            using (var afterStream = new MemoryStream(data))
                            {
                                hudLayout = Serializer.Deserialize<HudLayout>(afterStream);
                            }

                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                HudPainter.UpdateHud(hudLayout);
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, "HUD service failed to read data", e);
                    }                    
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "HUD service failed to establish connection", e);
            }

            LogProvider.Log.Info("HUD service has been stopped");
        }

        public void Dispose()
        {
            if (pipeClient != null)
            {
                pipeClient.Dispose();
            }
        }
    }
}