using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Files.Views.Popups
{
    /// <summary>
    /// Interaction logic for SettingsPopup.xaml
    /// </summary>
    public partial class SettingsPopup : UserControl
    {
        public SettingsPopup()
        {
            InitializeComponent();
        }

        private void Button_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var button = sender as Button;

            if (button.Visibility == Visibility.Visible)
            {
                button.Focus();
            }
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e) => e.Handled = true;
    }
}
