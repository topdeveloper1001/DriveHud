//-----------------------------------------------------------------------
// <copyright file="SubPacket.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using System;
using System.Collections.Generic;

namespace DriveHUD.Importers.PokerMaster
{
    internal class SubPacket<T> where T : class
    {
        public SubPacket(byte[] bytes, int expectedLength, DateTime createdDate)
        {
            Bytes = new List<byte>(bytes);
            DateReceived = DateTime.Now;
            ExpectedLength = expectedLength;
            CreatedDate = createdDate;
        }

        private List<byte> Bytes
        {
            get;
            set;
        }

        private DateTime DateReceived
        {
            get;
            set;
        }

        private DateTime CreatedDate
        {
            get;
            set;
        }

        private int ExpectedLength
        {
            get;
            set;
        }

        private int CurrentLength
        {
            get
            {
                return Bytes.Count;
            }
        }

        public void Add(byte[] bytes)
        {
            if (CurrentLength + bytes.Length > ExpectedLength)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), $"Length of bytes to add is out of range: {bytes.Length}, {CurrentLength}, {ExpectedLength}");
            }

            Bytes.AddRange(bytes);
        }

        public bool CanAddSubPacket(byte[] bytes, DateTime createdDate)
        {
            if (createdDate < CreatedDate)
            {
                return false;
            }

            return bytes.Length + CurrentLength <= ExpectedLength;
        }

        public bool CanCompleteBySubPacket(byte[] partialBytes, DateTime partialPacketCreatedDate)
        {
            if (partialPacketCreatedDate < CreatedDate)
            {
                return false;
            }

            return partialBytes.Length + CurrentLength == ExpectedLength;
        }

        public bool IsExpired(int expirationPeriod)
        {
            return (DateTime.Now - DateReceived).TotalMilliseconds > expirationPeriod;
        }

        public bool TryParse(int startingPosition, out T package)
        {
            package = null;

            if (ExpectedLength != CurrentLength)
            {
                return false;
            }

            return SerializationHelper.TryDeserialize(Bytes.ToArray(), startingPosition, out package);
        }
    }
}