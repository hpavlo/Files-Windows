using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Files.Views
{
    internal class FileListView : ListView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FileListViewItem();
        }
    }

    internal class FileListViewItem : ListViewItem
    {
        private bool wasDown = false;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) => wasDown = true;

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (wasDown) base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) { }
    }
}
