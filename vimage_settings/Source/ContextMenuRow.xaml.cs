using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using vimage.Common;
using Action = vimage.Common.Action;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for ContextMenuRow.xaml
    /// </summary>
    public partial class ContextMenuRow : UserControl
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

        public ContextMenuRow()
        {
            InitializeComponent();
            SetFunctions();
        }

        public ContextMenuRow(
            string name,
            ActionFunc? func,
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

            var actionName = "";
            if (func is ActionEnum actionEnum)
                actionName = actionEnum.Value.ToString();
            else if (func is CustomAction customAction)
                actionName = customAction.Value;

            int funcIndex = ItemFunction.Items.IndexOf(actionName);
            if (funcIndex != -1)
                ItemFunction.SelectedIndex = funcIndex;
        }

        public ContextMenuItem GetAsContextMenuItem()
        {
            var actionName = ItemFunction.Text.Trim();
            ActionFunc? func;

            if (actionName == null || actionName.Length <= 0)
                func = null;
            else  if (Enum.TryParse<Action>(actionName, true, out var action))
                func = new ActionEnum(action);
            else
                func = new CustomAction(ItemFunction.Text.Trim());

            return new ContextMenuItem(ItemName.Text, func);
        }

        public void SetFunctions()
        {
            int prevIndex = -1;
            if (ItemFunction.Items.Count != 0)
                prevIndex = ItemFunction.SelectedIndex;

            ItemFunction.Items.Clear();

            _ = ItemFunction.Items.Add("-");
            for (int i = 0; i < Actions.MenuActions.Length; i++)
                _ = ItemFunction.Items.Add(Actions.MenuActions[i].ToString());
            if (App.Config != null)
            {
                CustomActionsStartIndex = ItemFunction.Items.Count;
                for (int i = 0; i < App.Config.CustomActions.Count; i++)
                    ItemFunction.Items.Add(App.Config.CustomActions[i].Name);
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
                i < CustomActionsStartIndex + App.Config.CustomActions.Count;
                i++
            )
            {
                if (i >= ItemFunction.Items.Count)
                {
                    // add
                    ItemFunction.Items.Add(
                        App.Config.CustomActions[i - CustomActionsStartIndex].Name
                    );
                }
                else
                {
                    // update name
                    ItemFunction.Items[i] = App.Config
                        .CustomActions[i - CustomActionsStartIndex]
                        .Name;
                }
            }
            while (
                ItemFunction.Items.Count > CustomActionsStartIndex + App.Config.CustomActions.Count
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
            ContextMenuEditor?.CurrentItemSelection = this;
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
                        if (itemAbove is ContextMenuRow itemAboveItem)
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

        public int Indent
        {
            get;
            set
            {
                field = value;
                IndentColumn.Width = new GridLength(field * 30);
            }
        }

        public bool Dragging
        {
            get;
            set
            {
                if (field == value)
                    return;

                if (ParentPanel is null || ParentCanvas is null)
                {
                    field = value;
                    return;
                }

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

                field = value;
            }
        }
    }
}
