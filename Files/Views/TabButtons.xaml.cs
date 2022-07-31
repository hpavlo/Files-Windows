using Files.Models;
using Files.ViewModels;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Files.Views
{
    /// <summary>
    /// Interaction logic for TabButtons.xaml
    /// </summary>
    public partial class TabButtons : UserControl
    {
        public TabButtons()
        {
            InitializeComponent();
        }

        private void ButtonList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = ListViewExtenders.GetFirstVisualChild<ScrollViewer>(ButtonList);

            if (e.Delta > 0) scrollViewer.LineLeft();
            else scrollViewer.LineRight();
        }
        private void TabButtonBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var disconnectedItem = typeof(BindingExpressionBase)
                .GetField("DisconnectedItem", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);

            if (e.ChangedButton == MouseButton.Middle && ((Border)sender).DataContext != disconnectedItem)
                ((TabButtonsViewModel)DataContext).RemoveTab(((TabButton)((Border)sender).DataContext).Index);
        }
        private void TabButtonBorder_DragEnter(object sender, DragEventArgs e)
        {
            ButtonList.SelectedItem = ((Border)sender).DataContext;
        }

        private void AddButton_Drop(object sender, DragEventArgs e)
        {
            ((TabButtonsViewModel)DataContext).AddTab((string[])e.Data.GetData(DataFormats.FileDrop), false);
        }
    }
}
