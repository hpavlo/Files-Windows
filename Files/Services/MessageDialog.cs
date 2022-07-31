using Files.Enums;
using Files.Views.Dialogs;
using System.Windows;

namespace Files.Services
{
    public class MessageDialog
    {
        public static MessageDialogResult Show(string messageText, string caption = "",
                                               MessageDialogButton button = MessageDialogButton.OK,
                                               MessageDialogImage image = MessageDialogImage.None)
        {
            MessageDialogWindow dialog = new MessageDialogWindow();
            dialog.Owner = Application.Current.MainWindow;

            return dialog.Show(messageText, caption, button, image);
        }
    }
}
