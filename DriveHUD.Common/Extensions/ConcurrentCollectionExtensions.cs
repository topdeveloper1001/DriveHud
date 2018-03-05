﻿//-----------------------------------------------------------------------
// <copyright file="ConcurrentCollectionExtensions.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;

namespace DriveHUD.Common.Extensions
{
    public static class ConcurrentCollectionExtensions
    {
        public static void Clear<T>(this BlockingCollection<T> blockingCollection)
        {
            if (blockingCollection == null)
            {
                throw new ArgumentNullException(nameof(blockingCollection));
            }

            while (blockingCollection.Count > 0)
            {
                blockingCollection.TryTake(out T item);
            }
        }
    }
}