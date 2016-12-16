﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;
using System.IO;

namespace Model.Site
{
    public class BlackChipPokerConfiguration : AmericasCardroomConfiguration
    {
        public override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.BlackChipPoker;
            }
        }

        public override string[] GetHandHistoryFolders()
        {
            var folders = new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                Path.GetPathRoot(Environment.SystemDirectory)
            }.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct();

            var dirs = (from possibleFolder in folders
                        let folder = Path.Combine(possibleFolder, "BlackChipPoker", "profiles")
                        select folder).ToArray();

            return dirs;
        }
    }
}