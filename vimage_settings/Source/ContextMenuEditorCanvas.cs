using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace vimage_settings
{
    public class ContextMenuEditorCanvas : Canvas
    {
        public ContextMenuRow? MovingItem;
        public ContextMenuRow GhostItem = new();
        public Rectangle SelectionRect = new()
        {
            Height = 4,
            Fill = System.Windows.Media.Brushes.Black,
            Opacity = 0.5f,
        };
        public int InsertAtIndex = -1;

        public ContextMenuEditorCanvas()
            : base() { }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            MovingItem?.Dragging = false;
        }

        public void SetupGhost(ContextMenuRow item)
        {
            if (GhostItem.Parent != null)
                return;

            GhostItem.UpdateCustomActions();
            GhostItem.ItemName.Text = item.ItemName.Text;
            GhostItem.ItemFunction.Text = item.ItemFunction.Text;
            GhostItem.Indent = item.Indent;
            GhostItem.IsEnabled = false;
        }
    }
}
