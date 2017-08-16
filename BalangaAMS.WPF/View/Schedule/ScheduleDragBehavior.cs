using Telerik.Windows.Controls;

namespace BalangaAMS.WPF.View.Schedule
{
    public class ScheduleDragBehavior : ScheduleViewDragDropBehavior
    {
        public override bool CanStartDrag(DragDropState state)
        {
            return false;
        }

        public override bool CanDrop(DragDropState state)
        {
            return false;
        }

        public override bool CanStartResize(DragDropState state)
        {
            return false;
        }

        public override bool CanResize(DragDropState state)
        {
            return false;
        }
    }
}
