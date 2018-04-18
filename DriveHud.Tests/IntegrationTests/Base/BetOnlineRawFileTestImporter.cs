//-----------------------------------------------------------------------
// <copyright file="BetOnlineRawFileTestImporter.cs" company="Ace Poker Solutions">
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
using DriveHUD.Importers.BetOnline;
using HandHistories.Parser.Parsers;
using NSubstitute;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DriveHud.Tests.IntegrationTests.Base
{
    public class BetOnlineRawFileTestImporter : IFileTestImporter
    {
        public IEnumerable<ParsingResult> Import(IFileImporter fileImporter, string handHistoryFileContent, GameInfo gameInfo)
        {
            var progress = Substitute.For<IDHProgress>();

            var xDocument = XDocument.Parse(handHistoryFileContent);

            var handHistoryNodes = xDocument.Descendants("HandHistory");

            var result = new List<ParsingResult>();

            foreach (var handHistoryNode in handHistoryNodes)
            {
                var xml = handHistoryNode.ToString();
                var betOnlineXmlConverter = new BetOnlineXmlToIPokerXmlConverter();
                betOnlineXmlConverter.Initialize(xml);

                var convertedResult = betOnlineXmlConverter.Convert();

                result.AddRange(fileImporter.Import(convertedResult.ConvertedXml, progress, convertedResult.GameInfo));
            }

            return result;
        }
    }
}