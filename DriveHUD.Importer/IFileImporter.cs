//-----------------------------------------------------------------------
// <copyright file="IFileImporter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Progress;
using HandHistories.Parser.Parsers;
using System;
using System.Collections.Generic;
using System.IO;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Import hands service
    /// </summary>
    public interface IFileImporter
    {
        #region File mode

        /// <summary>
        /// Import hands history data 
        /// </summary>
        /// <param name="file">File to be imported</param>       
        /// <param name="progress">Progress object to report</param>     
        void Import(FileInfo file, IDHProgress progress);

        /// <summary>
        /// Import hands history data 
        /// </summary>
        /// <param name="files">Files to be imported</param>       
        /// <param name="progress">Progress object to report</param>     
        void Import(FileInfo[] files, IDHProgress progress);

        /// <summary>
        /// Import hands history data 
        /// </summary>
        /// <param name="directory">Directory to be imported</param>  
        /// <param name="progress">Progress object to report</param>           
        void Import(DirectoryInfo directory, IDHProgress progress);

        /// <summary>
        /// Import hands history data 
        /// </summary>
        /// <param name="text">Text to import</param>  
        /// <param name="progress">Progress object to report</param>           
        /// <param name="gameInfo">Game information</param> 
        /// <param name="rethrowInvalidHand">Rethrow invalid hands flag</param>
        IEnumerable<ParsingResult> Import(string text, IDHProgress progress, GameInfo gameInfo, bool rethrowInvalidHands);

        #endregion
    }
}