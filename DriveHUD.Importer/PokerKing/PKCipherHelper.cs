//-----------------------------------------------------------------------
// <copyright file="PKModelTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.PokerKing
{
    internal class PKCipherHelper
    {
        private static readonly byte[] cipherMode = new byte[] { 0x41, 0x45, 0x53, 0x2F, 0x45, 0x43, 0x42, 0x2F, 0x50, 0x4B, 0x43, 0x53,
            0x35, 0x50, 0x61, 0x64, 0x64, 0x69, 0x6E, 0x67 };

        public static byte[] Decode(byte[] key, byte[] body)
        {
            var cipher = CipherUtilities.GetCipher(Encoding.ASCII.GetString(cipherMode));
            cipher.Init(false, new KeyParameter(key));

            var bytes = cipher.ProcessBytes(body);
            var final = cipher.DoFinal();

            return (bytes == null) ? final :
                ((final == null) ? bytes : bytes.Concat(final).ToArray());
        }
    }
}