using FileExplorer.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileExplorer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.XButton2) (DataContext as MainViewModel).NextFolder();
            if (e.ChangedButton == MouseButton.XButton1) (DataContext as MainViewModel).PreviousFolder();
        }
    }
}
