using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Files.Views.Popups
{
    /// <summary>
    /// Interaction logic for InformationDialog.xaml
    /// </summary>
    public partial class RenamePopup : UserControl
    {
        public RenamePopup()
        {
            InitializeComponent();
        }

        private void TextBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox.Visibility == Visibility.Visible)
            {
                textBox.Focus();
                Keyboard.Focus(textBox);
                textBox.SelectAll();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PopupInvalidName.IsOpen)
                PopupInvalidName.IsOpen = false;
        }
    }
}
