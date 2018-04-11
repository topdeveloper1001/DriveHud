//-----------------------------------------------------------------------
// <copyright file="BasicFileTestImporter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Progress;
using DriveHUD.Importers;
using HandHistories.Parser.Parsers;
using NSubstitute;
using System.Collections.Generic;

namespace DriveHud.Tests.IntegrationTests.Base
{
    public class BasicFileTestImporter : IFileTestImporter
    {
        public IEnumerable<ParsingResult> Import(IFileImporter fileImporter, string handHistoryFileContent, GameInfo gameInfo)
        {
            var progress = Substitute.For<IDHProgress>();
            return fileImporter.Import(handHistoryFileContent, progress, gameInfo);
        }
    }
}