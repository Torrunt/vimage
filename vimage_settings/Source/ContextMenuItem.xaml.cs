using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using vimage.Common;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for ContextMenuItem.xaml
    /// </summary>
    public partial class ContextMenuItem : UserControl
    {
        private readonly ContextMenu? ContextMenuEditor;
        private readonly Panel? ParentPanel;
        private readonly ContextMenuEditorCanvas? ParentCanvas;
        private readonly ScrollViewer? ScrollViewer;
        private int CustomActionsStartIndex = -1;
        private Point AnchorPoint;

        private bool Selected = false;
        private readonly SolidColorBrush BrushSelected = new(Colors.DodgerBlue);
        private readonly SolidColorBrush BrushOver = new(Colors.LightSkyBlue);

        public ContextMenuItem()
        {
            InitializeComponent();
            SetFunctions();
        }

        public ContextMenuItem(
            string name,
            object func,
            ContextMenu contextMenu,
            Panel parentPanel,
            ContextMenuEditorCanvas canvas,
            ScrollViewer scroll,
            int indent = 0
        )
        {
            InitializeComponent();

            ContextMenuEditor = contextMenu;
            ParentPanel = parentPanel;
            ParentCanvas = canvas;
            ScrollViewer = scroll;
            Indent = indent;
            SetFunctions();

            ItemName.Text = name;

            int funcIndex = ItemFunction.Items.IndexOf(
                (string)(func is vimage.Common.Action action ? action.ToNameString() : func)
            );
            if (funcIndex != -1)
                ItemFunction.SelectedIndex = funcIndex;
        }

        public void SetFunctions()
        {
            int prevIndex = -1;
            if (ItemFunction.Items.Count != 0)
                prevIndex = ItemFunction.SelectedIndex;

            ItemFunction.Items.Clear();

            _ = ItemFunction.Items.Add("-");
            for (int i = 0; i < Actions.MenuActions.Length; i++)
                _ = ItemFunction.Items.Add(Actions.MenuActions[i].ToNameString());
            if (App.vimageConfig != null)
            {
                CustomActionsStartIndex = ItemFunction.Items.Count;
                for (int i = 0; i < App.vimageConfig.CustomActions.Count; i++)
                    ItemFunction.Items.Add(App.vimageConfig.CustomActions[i].name);
            }
            ItemFunction.SelectedIndex = 0;

            if (prevIndex != -1 && prevIndex < ItemFunction.Items.Count)
                ItemFunction.SelectedIndex = prevIndex;
        }

        public void UpdateCustomActions()
        {
            if (CustomActionsStartIndex == -1)
                return;

            int prevSelected = ItemFunction.SelectedIndex;

            for (
                int i = CustomActionsStartIndex;
                i < CustomActionsStartIndex + App.vimageConfig.CustomActions.Count;
                i++
            )
            {
                if (i >= ItemFunction.Items.Count)
                {
                    // add
                    ItemFunction.Items.Add(
                        App.vimageConfig.CustomActions[i - CustomActionsStartIndex].name
                    );
                }
                else
                {
                    // update name
                    ItemFunction.Items[i] = App.vimageConfig
                        .CustomActions[i - CustomActionsStartIndex]
                        .name;
                }
            }
            while (
                ItemFunction.Items.Count
                > CustomActionsStartIndex + App.vimageConfig.CustomActions.Count
            )
                ItemFunction.Items.RemoveAt(ItemFunction.Items.Count - 1); // delete

            if (ItemFunction.SelectedIndex < 0 && prevSelected < ItemFunction.Items.Count)
                ItemFunction.SelectedIndex = prevSelected;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ParentPanel?.Children.Remove(this);
            if (Application.Current.MainWindow is MainWindow mainWindow)
                _ = mainWindow.ContextMenuEditor.Items.Remove(this);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ParentCanvas?.MovingItem is null)
                UserControl.Background = Selected ? BrushSelected : BrushOver;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ParentCanvas?.MovingItem is null)
                UserControl.Background = Selected ? BrushSelected : new SolidColorBrush();
        }

        private void ItemName_GotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            SelectItem();
        }

        private void ItemName_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            UnselectItem();
        }

        private void Item_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectItem();
        }

        private void Item_LostFocus(object sender, RoutedEventArgs e)
        {
            UnselectItem();
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FocusManager.SetFocusedElement(UserControl, ItemName);
            _ = Keyboard.Focus(ItemName);
            ItemName.SelectAll();
            e.Handled = true;

            AnchorPoint = e.GetPosition(this);
        }

        public void UnselectItem()
        {
            if (ParentCanvas?.MovingItem is not null)
                return;

            Selected = false;
            UserControl.Background = new SolidColorBrush();
        }

        public void SelectItem(bool selectTextBox = false)
        {
            if (ParentCanvas?.MovingItem is not null)
                return;

            Selected = true;
            if (ContextMenuEditor is not null)
                ContextMenuEditor.CurrentItemSelection = this;
            UserControl.Background = BrushSelected;

            if (selectTextBox)
            {
                _ = Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                    (System.Threading.ThreadStart)
                        delegate()
                        {
                            _ = Focus();
                        }
                );
                FocusManager.SetFocusedElement(UserControl, ItemName);
                _ = Keyboard.Focus(ItemName);
                ItemName.SelectAll();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // start dragging
                if (
                    !Dragging
                    && Parent == ParentPanel
                    && ParentCanvas?.MovingItem is null
                    && e.GetPosition(null).X < (Indent + 1) * 30
                )
                    Dragging = true;

                if (Dragging && ParentCanvas is not null)
                {
                    // update position
                    var posX = e.GetPosition(ParentCanvas).X - AnchorPoint.X;
                    var posY = e.GetPosition(ParentCanvas).Y - AnchorPoint.Y;
                    var i = Math.Max(
                        (int)Math.Round((posY + ((MinHeight - 2) / 2)) / (MinHeight - 2)),
                        0
                    );
                    i = Math.Min(i, ParentPanel?.Children.Count ?? 0);

                    var top = Math.Max(i * (MinHeight - 2), 0);
                    Canvas.SetTop(ParentCanvas.SelectionRect, top - 2);

                    int indent = 0;
                    if (i != 0)
                    {
                        var itemAbove = ParentPanel?.Children[i - 1];
                        if (itemAbove is ContextMenuItem itemAboveItem)
                        {
                            if (posX >= (itemAboveItem.Indent + 1) * 30)
                                indent = itemAboveItem.Indent + 1;
                            else if (posX >= itemAboveItem.Indent * 30)
                                indent = itemAboveItem.Indent;
                        }
                    }
                    Canvas.SetLeft(ParentCanvas.SelectionRect, indent * 30);

                    Indent = indent;
                    ParentCanvas.InsertAtIndex = i;

                    Canvas.SetTop(this, posY);
                    Canvas.SetLeft(this, posX);

                    // scroll window when dragging past top/bottom
                    if (ScrollViewer is not null)
                    {
                        if (
                            posY + MinHeight
                            > ScrollViewer.ActualHeight + ScrollViewer.VerticalOffset
                        )
                        {
                            ScrollViewer.ScrollToVerticalOffset(
                                ScrollViewer.VerticalOffset
                                    + (
                                        posY
                                        - (
                                            ScrollViewer.ActualHeight
                                            + ScrollViewer.VerticalOffset
                                            - MinHeight
                                        )
                                    )
                            );
                        }
                        else if (posY < ScrollViewer.VerticalOffset)
                        {
                            ScrollViewer.ScrollToVerticalOffset(
                                ScrollViewer.VerticalOffset - (ScrollViewer.VerticalOffset - posY)
                            );
                        }
                    }

                    e.Handled = true;
                }
            }
            else if (Dragging)
                Dragging = false;
        }

        private int _Indent = 0;
        public int Indent
        {
            get { return _Indent; }
            set
            {
                _Indent = value;
                IndentColumn.Width = new GridLength(_Indent * 30);
            }
        }

        private bool _Dragging = false;
        public bool Dragging
        {
            get { return _Dragging; }
            set
            {
                if (_Dragging == value)
                    return;
                if (!Dragging)
                {
                    int index = ParentPanel.Children.IndexOf(this);
                    ParentPanel.Children.Remove(this);
                    _ = ParentCanvas.Children.Add(this);

                    ParentCanvas.SetupGhost(this);
                    ParentPanel.Children.Insert(index, ParentCanvas.GhostItem);
                    ParentCanvas.MovingItem = this;
                    ParentCanvas.InsertAtIndex = index;

                    Width = ParentPanel.ActualWidth;

                    ParentCanvas.SelectionRect.Width = Width;
                    _ = ParentCanvas.Children.Add(ParentCanvas.SelectionRect);

                    Selected = true;
                    Opacity = 0.7f;

                    _ = CaptureMouse();
                    ItemName.IsEnabled = false;
                    ItemFunction.IsEnabled = false;
                    ButtonDelete.IsEnabled = false;
                }
                else
                {
                    int ghostIndex = ParentPanel.Children.IndexOf(ParentCanvas.GhostItem);
                    if (ParentCanvas.InsertAtIndex == ghostIndex + 1)
                        ParentCanvas.InsertAtIndex = ghostIndex;

                    if (ParentCanvas.InsertAtIndex > ghostIndex)
                        ParentCanvas.InsertAtIndex--;

                    ParentPanel.Children.Remove(ParentCanvas.GhostItem);
                    ParentCanvas.Children.Remove(this);
                    ParentCanvas.Children.Remove(ParentCanvas.SelectionRect);

                    if (ParentCanvas.InsertAtIndex != -1)
                    {
                        if (ParentCanvas.InsertAtIndex >= ParentPanel.Children.Count)
                            _ = ParentPanel.Children.Add(this);
                        else
                            ParentPanel.Children.Insert(ParentCanvas.InsertAtIndex, this);
                    }
                    ParentCanvas.MovingItem = null;
                    ParentCanvas.InsertAtIndex = -1;

                    Width = double.NaN;

                    Opacity = 1;
                    UnselectItem();

                    ReleaseMouseCapture();
                    ItemName.IsEnabled = true;
                    ItemFunction.IsEnabled = true;
                    ButtonDelete.IsEnabled = true;
                }

                _Dragging = value;
            }
        }
    }
}
