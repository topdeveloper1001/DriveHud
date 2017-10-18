//-----------------------------------------------------------------------
// <copyright file="CertificateHelper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.WinApi;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace DriveHUD.Common.Utils
{
    public class CertificateHelper
    {
        public static bool Verify(string assemblyPath, X509Certificate2 assemblyCertificate = null)
        {
#if DEBUG
            return true;
#endif

            try
            {
                var isVerified = WinTrust.VerifyEmbeddedSignature(assemblyPath, WinVerifyTrustResult.Success);

                X509Certificate2 cert = new X509Certificate2(assemblyPath);

                if (assemblyCertificate == null)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    assemblyCertificate = new X509Certificate2(assembly.Location);
                }

                return isVerified && cert.Equals(assemblyCertificate);
            }
            catch
            {
            }

            return false;
        }
    }
}