//-----------------------------------------------------------------------
// <copyright file="IPokerHandExportPreparingService.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Export
{
    internal class IPokerHandExportPreparingService : CommonHandExportPreparingService
    {
        private const string IgntionFastPokerPrefix = "Zone Poker";
        private const string BetOnlineFastPokerPrefix = "Fast Fold";
        private const string IPokerFastPokerPrefix = "Speed";

        private Dictionary<string, string> sessionToFile = new Dictionary<string, string>();
        private Dictionary<FastFoldKey, string> fastFoldSessions = new Dictionary<FastFoldKey, string>();

        protected override string HandHistoryFilePatternName => "dh_exported_file_{0}.xml";

        private bool TryParseFastFoldPokerPrefix(string hand, EnumPokerSites site, out string adjustedHand)
        {
            adjustedHand = null;

            var networks = EntityUtils.GetNetworkSites();

            if (networks[EnumPokerNetworks.Ignition].Contains(site) &&
                hand.ContainsIgnoreCase(IgntionFastPokerPrefix))
            {
                adjustedHand = hand.Replace(IgntionFastPokerPrefix, IPokerFastPokerPrefix);
                return true;
            }

            if ((networks[EnumPokerNetworks.Chico].Contains(site) ||
                networks[EnumPokerNetworks.QuadnetIndia].Contains(site)) && hand.ContainsIgnoreCase(BetOnlineFastPokerPrefix))
            {
                adjustedHand = hand.Replace(BetOnlineFastPokerPrefix, IPokerFastPokerPrefix);
                return true;
            }

            return false;
        }

        public override void WriteHandsToFile(string folder, IEnumerable<string> hands, EnumPokerSites site)
        {
            var filesStreamWriters = new Dictionary<string, StreamWriter>();

            foreach (var hand in hands)
            {
                var handInfo = PrepareHandInfo(hand, site);

                StreamWriter streamWriter;

                if (!sessionToFile.TryGetValue(handInfo.Session, out string file))
                {
                    file = CreateFileName(folder);

                    fileCounter++;

                    sessionToFile.Add(handInfo.Session, file);

                    streamWriter = new StreamWriter(file, false, Encoding.UTF8);
                    streamWriter.Write(handInfo.HandText);
                    streamWriter.Flush();
                    streamWriter.BaseStream.Seek(-10, SeekOrigin.End);

                    filesStreamWriters.Add(file, streamWriter);

                    continue;
                }

                if (!filesStreamWriters.TryGetValue(file, out streamWriter))
                {
                    FileStream fileStream;

                    try
                    {
                        fileStream = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                    }
                    catch
                    {
                        // try to open it again
                        Task.Delay(500).Wait();
                        fileStream = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                    }

                    streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
                    streamWriter.BaseStream.Seek(-10, SeekOrigin.End);

                    filesStreamWriters.Add(file, streamWriter);
                }

                streamWriter.WriteLine(handInfo.GameText);
            }

            filesStreamWriters.ForEach(x =>
            {
                x.Value.WriteLine("</session>");
                x.Value.Close();
                x.Value.Dispose();
            });
        }

        private HandInfo PrepareHandInfo(string hand, EnumPokerSites site)
        {
            // insert pokersite tag
            if (!hand.ContainsIgnoreCase("<pokersite"))
            {
                var closeGeneralTagIndex = hand.IndexOf("</general>", StringComparison.OrdinalIgnoreCase);

                if (closeGeneralTagIndex > 0)
                {
                    hand = hand.Insert(closeGeneralTagIndex, $"<pokersite>{site}</pokersite>{Environment.NewLine}");
                }
            }

            var handInfo = new HandInfo();

            // need to adjust fast fold hands
            if (TryParseFastFoldPokerPrefix(hand, site, out string adjustedHand))
            {
                hand = AdjustFastFoldHand(adjustedHand);
                handInfo.IsFastFold = true;
            }

            handInfo.Session = hand.TakeBetween("sessioncode=\"", "\"", stringComparison: StringComparison.OrdinalIgnoreCase);
            handInfo.GameType = hand.TakeBetween("<gametype>", "</gametype>", stringComparison: StringComparison.OrdinalIgnoreCase);
            handInfo.TableName = hand.TakeBetween("<tablename>", "</tablename>", stringComparison: StringComparison.OrdinalIgnoreCase);
            handInfo.HandText = hand;
            handInfo.GameText = hand.TakeBetween("</general>", "</session>", stringComparison: StringComparison.OrdinalIgnoreCase).Trim();

            if (handInfo.IsSessionEmpty && handInfo.IsFastFold)
            {
                var fastFoldKey = new FastFoldKey
                {
                    GameType = handInfo.GameType,
                    TableName = handInfo.TableName
                };

                if (!fastFoldSessions.TryGetValue(fastFoldKey, out string session))
                {
                    var random = RandomProvider.GetThreadRandom();
                    session = random.Next(100000, 999999).ToString();

                    fastFoldSessions.Add(fastFoldKey, session);
                }

                handInfo.Session = session;
            }

            return handInfo;
        }

        private string AdjustFastFoldHand(string hand)
        {
            var adjuster = ServiceLocator.Current.GetInstance<IZoneHandAdjuster>();
            var adjustedHand = adjuster.Adjust(hand);
            return adjustedHand;
        }

        private class HandInfo
        {
            public string Session { get; set; }

            public string GameType { get; set; }

            public string TableName { get; set; }

            public bool IsFastFold { get; set; }

            public string HandText { get; set; }

            public string GameText { get; set; }

            public bool IsSessionEmpty
            {
                get
                {
                    return string.IsNullOrEmpty(Session) || Session == "0";
                }
            }
        }

        private class FastFoldKey
        {
            public string GameType { get; set; }

            public string TableName { get; set; }

            public override bool Equals(object obj)
            {
                return Equals(obj as FastFoldKey);
            }

            public bool Equals(FastFoldKey key)
            {
                return key != null && key.GameType == GameType && key.TableName == TableName;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + GameType.GetHashCode();
                    hashcode = (hashcode * 31) + TableName.GetHashCode();
                    return hashcode;
                }
            }
        }
    }
}