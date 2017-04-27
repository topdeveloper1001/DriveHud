//-----------------------------------------------------------------------
// <copyright file="RadListBoxDragDropBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Stats;
using System.Collections.Generic;
using System.Linq;
using Telerik.Windows.DragDrop.Behaviors;

namespace DriveHUD.Application.Controls
{
    public class RadListBoxDragDropBehavior : ListBoxDragDropBehavior
    {
        public RadListBoxDragDropBehavior()
            : base()
        {            
        }

        protected override IEnumerable<object> CopyDraggedItems(DragDropState state)
        {            
            return base.CopyDraggedItems(state);
        }

        public override void Drop(DragDropState state)
        {            
            if (!state.IsSameControl && state.DraggedItems.Cast<StatInfo>().All(x => x is StatInfoBreak))
            {
                base.DragDropCanceled(state);
                return;
            }

            base.Drop(state);
        }        

        public override void DragDropCanceled(DragDropState state)
        {            
            if (!state.DraggedItems.Cast<StatInfo>().All(x => x is StatInfoBreak))
            {
                base.DragDropCanceled(state);
            }
            else
            {
                base.DragDropCompleted(state);
            }
        }

        public override void DragDropCompleted(DragDropState state)
        {           
            base.DragDropCompleted(state);
        }
    }
}