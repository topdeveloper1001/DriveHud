//-----------------------------------------------------------------------
// <copyright file="RequestUtils.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Text;

namespace DriveHUD.Importers.GGNetwork.Network
{
    internal class RequestUtils
    {
        private static readonly Random rnd = new Random();

        public static string GetProtocolVersion()
        {
            return "a4e41d3c-aa3d-e10f-7b50-80559cff640e";
        }

        public static string CreateFakeHardwareSerialNumber()
        {
            const string pattern = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx";

            var sb = new StringBuilder();

            var rnd = new Random();

            foreach (char t in pattern)
            {
                switch (t)
                {
                    case 'x':
                        switch (rnd.Next(0, 2))
                        {
                            case 0:
                                sb.Append(GetRandomChar());
                                break;
                            case 1:
                                sb.Append(GetRandomNumber());
                                break;
                        }
                        break;
                    case '4':
                        sb.Append('4');
                        break;
                    case '-':
                        sb.Append('-');
                        break;
                }
            }
            return sb.ToString();
        }

        public static string CreateFakeMacAddress()
        {
            const string pattern = "xx:xx:xx:xx:xx:xx";

            StringBuilder sb = new StringBuilder();

            foreach (char t in pattern)
            {
                switch (t)
                {
                    case 'x':
                        {
                            sb.Append(GetRandomChar());
                            break;
                        }
                    case ':':
                        {
                            sb.Append(':');
                            break;
                        }
                }
            }

            return sb.ToString();
        }

        private static char GetRandomChar()
        {
            const string chars = "ABCDEF0123456789";
            int value = rnd.Next(0, chars.Length);
            return char.ToLower(chars[value]);
        }

        public static int GetRandomNumber()
        {
            return rnd.Next(0, 9);
        }
    }
}