//-----------------------------------------------------------------------
// <copyright file="ObjectsHelper.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.DataTypes
{
    public static class ObjectsHelper
    {
        public static int GetNextID(IList<NoteObject> list)
        {
            if (list == null || list.Count == 0)
            {
                return 1;
            }

            var id = list.Max(x => x.ID) + 1;

            return id;
        }
    }
}