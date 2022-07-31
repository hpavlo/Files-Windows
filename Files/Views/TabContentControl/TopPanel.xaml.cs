using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Files.Views.TabContentControl
{
    /// <summary>
    /// Interaction logic for TopPanel.xaml
    /// </summary>
    public partial class TopPanel : UserControl
    {
        public TopPanel()
        {
            InitializeComponent();
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = ListViewExtenders.GetFirstVisualChild<ScrollViewer>((DependencyObject)sender);

            if (e.Delta > 0) scrollViewer.LineLeft();
            else scrollViewer.LineRight();
        }

        private void Border_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(PathTextBox), PathTextBox);
        }

        private void Border_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((Border)sender).Visibility == Visibility.Collapsed)
                PathTextBox.CaretIndex = PathTextBox.Text.Length;
        }
    }
}
