﻿//-----------------------------------------------------------------------
// <copyright file="HandWinners.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Model.Solvers
{
    public class HandWinners
    {
        public IEnumerable<int> Hi { get; set; } = new List<int>();

        public IEnumerable<int> Lo { get; set; } = new List<int>();

        public IEnumerable<int> All
        {
            get
            {
                return Hi != null && Lo != null ?
                        Hi.Concat(Lo).Distinct() :
                        (Hi ?? Lo);
            }
        }
    }
}