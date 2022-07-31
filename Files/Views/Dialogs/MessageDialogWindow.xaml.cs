using Files.Enums;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Files.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialogWindow : Window
    {
        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern long DwmSetWindowAttribute(IntPtr hwnd, uint attribute, ref uint pvAttribute, uint cbAttribute);

        private uint DWMWA_WINDOW_CORNER_PREFERENCE = 33;
        private uint DWMWCP_ROUND = 2;

        /// <summary>
        /// Result of dialog
        /// </summary>
        public MessageDialogResult Result { get; private set; }

        public MessageDialogWindow()
        {
            InitializeComponent();

            //Window corner round
            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            DwmSetWindowAttribute(hWnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref DWMWCP_ROUND, sizeof(uint));

            //If main window is always on top, dialog to always on top
            Topmost = Application.Current.MainWindow.Topmost;
        }

        public MessageDialogResult Show(string messageText, string caption,
                          MessageDialogButton button,
                          MessageDialogImage image)
        {
            MessageText.Text = messageText;
            Caption.Text = caption;

            SelectButton(button);
            SelectImage(image);

            ShowDialog();

            return Result;
        }

        private void SelectButton(MessageDialogButton button)
        {
            switch (button)
            {
                case MessageDialogButton.OK:
                    ShowButton(false, false, true, false);
                    OKButton.Focus();
                    break;
                case MessageDialogButton.OKCancel:
                    ShowButton(false, false, true, true);
                    OKButton.Focus();
                    break;
                case MessageDialogButton.YesNo:
                    ShowButton(true, true, false, false);
                    YesButton.Focus();
                    break;
                case MessageDialogButton.YesNoCancel:
                    ShowButton(true, true, false, true);
                    YesButton.Focus();
                    break;
            }
        }

        private void SelectImage(MessageDialogImage image)
        {
            switch (image)
            {
                case MessageDialogImage.None:
                    MessageImage.Visibility = Visibility.Collapsed;
                    break;
                case MessageDialogImage.Stop:
                    MessageImage.Source = (DrawingImage)FindResource("StopIcon");
                    break;
                case MessageDialogImage.Warning:
                    MessageImage.Source = (DrawingImage)FindResource("WarningIcon");
                    break;
                case MessageDialogImage.Question:
                    MessageImage.Source = (DrawingImage)FindResource("QuestionIcon");
                    break;
                case MessageDialogImage.Information:
                    MessageImage.Source = (DrawingImage)FindResource("InformationIcon");
                    break;
            }
        }

        private void ShowButton(bool yesButton, bool noButton, bool okButton, bool cancelButton)
        {
            YesButton.Visibility = yesButton ? Visibility.Visible : Visibility.Collapsed;
            NoButton.Visibility = noButton ? Visibility.Visible : Visibility.Collapsed;
            OKButton.Visibility = okButton ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = cancelButton ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SendResult(MessageDialogResult result)
        {
            Result = result;
            Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => SendResult(MessageDialogResult.None);

        private void YesButton_Click(object sender, RoutedEventArgs e) => SendResult(MessageDialogResult.Yes);

        private void NoButton_Click(object sender, RoutedEventArgs e) => SendResult(MessageDialogResult.No);

        private void OKButton_Click(object sender, RoutedEventArgs e) => SendResult(MessageDialogResult.OK);

        private void CancelButton_Click(object sender, RoutedEventArgs e) => SendResult(MessageDialogResult.Cancel);
    }
}
