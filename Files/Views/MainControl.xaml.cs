using Files.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace Files.Views
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainControl : UserControl
    {
        public MainControl()
        {
            InitializeComponent();
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.XButton1)
                ((MainViewModel)DataContext).Tabs[((MainViewModel)DataContext).SelectedTabIndex].CenterPanelViewModel.PreviousPath();

            if (e.ChangedButton == MouseButton.XButton2)
                ((MainViewModel)DataContext).Tabs[((MainViewModel)DataContext).SelectedTabIndex].CenterPanelViewModel.NextPath();
        }
    }
}
