//-----------------------------------------------------------------------
// <copyright file="HandNumberTableName.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Enums;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Hand header class
    /// </summary>
    internal class HandNumberTableName
    {
        public ulong HandNumber { get; set; }

        public decimal Rake { get; set; }

        public decimal BuyIn { get; set; }

        public int TableType { get; set; }

        public uint TournamentID { get; set; }

        public uint TableID { get; set; }

        public int WindowHandle { get; set; }

        public string TableName { get; set; }

        public bool IsTournament { get; set; }

        public bool HeroWasMoved { get; set; }

        public GameFormat GameFormat { get; set; }

        public int LastNotHeroPlayerFinishedPlace { get; set; }

        public HandNumberTableName()
        {
            this.HandNumber = 0;
            this.Rake = 0.0m;
            this.BuyIn = 0.0m;
            this.TableType = 0;
            this.TournamentID = 0;
            this.TableID = 0;
            this.WindowHandle = 0;
            this.TableName = string.Empty;
            this.IsTournament = false;
        }

        public override string ToString()
        {
            return string.Format("HandNumber: {0}; TableName: {1}; TableID: {2}; IsTournament: {3}; TournamentID: {4}; TableType: {5}; GameFormat: {10}; WindowHandle: {6}; BuyIn: {7}; Rake: {8}; HeroWasMoved: {9}; LastPlayerFinishedPlace: {11}",
                    HandNumber, TableName, TableID, IsTournament, TournamentID, TableType, WindowHandle, BuyIn, Rake, HeroWasMoved, GameFormat, LastNotHeroPlayerFinishedPlace);
        }
    }
}