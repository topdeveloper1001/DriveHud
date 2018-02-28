//-----------------------------------------------------------------------
// <copyright file="BodyDecryptor.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace DriveHUD.Importers.PokerMaster
{
    /// <summary>
    /// Class to decrypt body of PM packages
    /// </summary>
    internal class BodyDecryptor
    {
        private static readonly byte[] defaultKey = new byte[] { 49, 49, 54, 102, 102, 53, 56, 99, 48, 98, 49, 55, 56, 52, 50, 57 };

        public BodyDecryptor()
        {
            LoadBdecLibrary();
        }

        public byte[] Decrypt(byte[] body)
        {
            return Decrypt(body, defaultKey, true);
        }

        public byte[] Decrypt(byte[] body, byte[] key, bool isLoggingEnabled)
        {
            var numArray = new byte[body.Length * 2];

            int length = body.Length * 2;

            var bodyToDecrypt = body.Select(x => (byte)(255 - x)).ToArray();

            if (Decrypt(bodyToDecrypt, body.Length, key, numArray, ref length) != 0 && isLoggingEnabled)
            {
                LogProvider.Log.Error(this, "Body wasn't decypted correctly.");
            }

            return numArray.Take(length).ToArray();
        }

        private void LoadBdecLibrary()
        {
#if DEBUG
            var decoderLib = Path.Combine(Environment.CurrentDirectory, "bdec.dll");
#else
            var decoderLib = Path.Combine(Environment.CurrentDirectory, "bin", "bdec.dll");
#endif

            var ws2_32dll = Path.Combine(Environment.SystemDirectory, "ws2_32.dll");
            var vcruntime140dll = Path.Combine(Environment.SystemDirectory, "vcruntime140.dll");

            if (!File.Exists(ws2_32dll))
            {
                LogProvider.Log.Error($"Library {ws2_32dll} has not been found");
            }

            if (!File.Exists(vcruntime140dll))
            {
                LogProvider.Log.Error($"Library {vcruntime140dll} has not been found");
            }

            if (!File.Exists(decoderLib))
            {
                throw new FileNotFoundException(decoderLib);
            }

            if (WinApi.LoadLibraryEx(decoderLib, IntPtr.Zero, 0) == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        [DllImport("bdec.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern int Decrypt(byte[] encryptedBytes, int encryptedBytesLengths, byte[] keyBytes, byte[] result, ref int resultLength);
    }
}