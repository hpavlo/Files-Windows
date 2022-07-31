using Files.Styles;
using Files.ViewModels;
using System;

namespace Files.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FilesWindow
    {
        public MainWindow()
        {
            //Set light theme
            Themes.SetTheme(Properties.Settings.Default.DarkMode ? Theme.Dark : Theme.Light, false);

            InitializeComponent();
            ((MainWindowViewModel)DataContext).CloseWindow = new Action(this.Close);
        }

        private void FilesWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == System.Windows.WindowState.Maximized)
                ((MainWindowViewModel)DataContext).ChangedWindowState(RestoreBounds);
        }

        private void FilesWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((MainWindowViewModel)DataContext).SaveSettings();
        }
    }
}
