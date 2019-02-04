//-----------------------------------------------------------------------
// <copyright file="CommonHandExportPreparingService.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Model.Export
{
    internal class CommonHandExportPreparingService : IHandExportPreparingService
    {
        protected int fileCounter = 1;
        protected int handsWrittenToFile = 0;

        protected const int HandsPerFile = 1000;

        protected virtual string HandHistoryFilePatternName => "dh_exported_file_{0}.txt";

        public virtual string PrepareHand(string hand, EnumPokerSites site)
        {
            return hand;
        }

        public virtual void WriteHandsToFile(string folder, IEnumerable<string> hands, EnumPokerSites site)
        {
            var file = CreateFileName(folder);

            var append = handsWrittenToFile != 0;

            StreamWriter streamWriter = null;

            try
            {
                streamWriter = new StreamWriter(file, append, Encoding.UTF8);

                foreach (var hand in hands)
                {
                    var preparedHand = PrepareHand(hand, site);

                    streamWriter.WriteLine(preparedHand);
                    streamWriter.WriteLine();
                    streamWriter.WriteLine();

                    handsWrittenToFile++;

                    if (handsWrittenToFile >= HandsPerFile)
                    {
                        handsWrittenToFile = 0;
                        fileCounter++;

                        file = CreateFileName(folder);

                        streamWriter.Close();
                        streamWriter = new StreamWriter(file, false, Encoding.UTF8);
                    }
                }
            }
            finally
            {
                streamWriter?.Dispose();
            }
        }

        protected virtual string CreateFileName(string folder)
        {
            var file = Path.Combine(folder, string.Format(HandHistoryFilePatternName, fileCounter));

            if (File.Exists(file))
            {
                fileCounter++;
                return CreateFileName(folder);
            }

            return file;
        }
    }
}