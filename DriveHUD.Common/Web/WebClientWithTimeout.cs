//-----------------------------------------------------------------------
// <copyright file="WebClientWithTimeout.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Net;

namespace DriveHUD.Common.Web
{
    /// <summary>
    /// Extended web client with timeout support
    /// </summary>
    public class WebClientWithTimeout : WebClient
    {
        /// <summary>
        /// Timeout for web request, default value is 10 sec
        /// </summary>
        public int Timeout { get; set; }

        public WebClientWithTimeout()
        {
            Timeout = 10000;
        }

        public WebClientWithTimeout(int timeout)
        {
            Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var objWebRequest = base.GetWebRequest(address);
            objWebRequest.Timeout = Timeout;
            return objWebRequest;
        }
    }
}