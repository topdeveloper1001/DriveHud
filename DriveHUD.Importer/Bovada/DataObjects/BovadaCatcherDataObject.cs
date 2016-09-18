//-----------------------------------------------------------------------
// <copyright file="BovadaCatcherDataObject.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Class represents base Bovada stream data object
    /// </summary>
    public class BovadaCatcherDataObject
    {
        /// <summary>
        /// Unique id of stream
        /// </summary>
        public string uid { get; set; }

        /// <summary>
        /// Handle of table window
        /// </summary>
        public string handle { get; set; }

        /// <summary>
        /// Hand number
        /// </summary>
        public string handnumber { get; set; }

        /// <summary>
        /// Url of table page (contains usefull information)
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// Command 
        /// </summary>
        public BovadaCommandDataObject cmd { get; set; }
    }
}