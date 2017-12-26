//-----------------------------------------------------------------------
// <copyright file="IgnitionTableData.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Importers.Bovada
{
    internal class IgnitionTableData
    {
        public uint Id { get; set; }

        public int TableSize { get; set; }

        public string TableName { get; set; }

        public GameFormat GameFormat { get; set; }

        public GameType GameType { get; set; }

        public GameLimit GameLimit { get; set; }

        public bool IsZone { get; set; }

        public override string ToString()
        {
            return $"TableId: {Id} TableSize: {TableSize} GameFormat: {GameFormat} GameType: {GameType} GameLimit: {GameLimit} TableName: {TableName} Zone: {IsZone}";
        }
    }
}