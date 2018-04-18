//-----------------------------------------------------------------------
// <copyright file="Command.cs" company="Ace Poker Solutions">
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
    /// Command 
    /// </summary>
    internal class Command
    {
        public ulong HandNumber { get; set; }

        public int CommandCode { get; set; }

        public CommandCodeEnum CommandCodeEnum { get; set; }

        public object CommandObject { get; set; }

        public Command()
        {
            this.HandNumber = 0;
            this.CommandCode = -1;
            this.CommandObject = null;
            this.CommandCodeEnum = CommandCodeEnum.None;
        }

        public Command(ulong handNumber, int commandCode, CommandCodeEnum commandCodeEnum)
        {
            this.HandNumber = handNumber;
            this.CommandCode = commandCode;
            this.CommandObject = null;
            this.CommandCodeEnum = commandCodeEnum;
        }

#if DEBUG
        public override string ToString()
        {
            return $"[{CommandCodeEnum.ToString().ToUpper()}]: {CommandObject} [{HandNumber}]";
        }
#endif
    }
}