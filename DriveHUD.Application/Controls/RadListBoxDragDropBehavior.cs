using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Telerik.Windows.Controls;
using Telerik.Windows.DragDrop.Behaviors;

using DriveHUD.ViewModels;
using DriveHUD.Application.ViewModels;

namespace DriveHUD.Application.Controls
{
    public class RadListBoxDragDropBehavior : ListBoxDragDropBehavior
    {
        public RadListBoxDragDropBehavior()
            : base()
        {

            Console.WriteLine("RadListBoxDragDropBehavior");
        }

        protected override IEnumerable<object> CopyDraggedItems(DragDropState state)
        {

            Console.WriteLine("CopyDraggedItems");
            return base.CopyDraggedItems(state);
        }

        public override void Drop(DragDropState state)
        {
            Console.WriteLine("Drop");
            if (!state.IsSameControl && state.DraggedItems.Cast<StatInfo>().All<StatInfo>(x => x is StatInfoBreak))
            {
                base.DragDropCanceled(state);
                return;
            }
            base.Drop(state);
        }


        public override void DragDropCanceled(DragDropState state)
        {
            Console.WriteLine("DragDropCanceled");
            if (!state.DraggedItems.Cast<StatInfo>().All<StatInfo>(x => x is StatInfoBreak))
            {
                base.DragDropCanceled(state);
            }
            else
                base.DragDropCompleted(state);
        }


        public override void DragDropCompleted(DragDropState state)
        {
            Console.WriteLine("DragDropCompleted");
            base.DragDropCompleted(state);

        }

    }

}
