using Files.Resources.MultilingualResources;
using Files.ViewModels.Base;
using System;
using System.Globalization;
using System.Windows;

namespace Files.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        public MainViewModel MainViewModel { get; set; }
        public PopupViewModel PopupViewModel { get; set; }

        public TabContentViewModel SelectedTabContentViewModel => MainViewModel.SelectedTabContentViewModel;
        public void SelectedTabChanged() => OnPropertyChanged(nameof(SelectedTabContentViewModel));

        private WindowState _windowState = Properties.Settings.Default.WindowMaximized ? WindowState.Maximized : WindowState.Normal;
        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                if (Set(ref _windowState, value))
                    Properties.Settings.Default.WindowMaximized = value == WindowState.Maximized;
            }
        }

        public Action CloseWindow { get; set; }

        public MainWindowViewModel()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.Language))
                Properties.Settings.Default.Language = CultureInfo.CurrentCulture.Name;
            TranslationSource.Instance.CurrentCulture = CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);

            //Initialize string collection for lase opened tabs
            if (Properties.Settings.Default.LastTabsCollection == null)
                Properties.Settings.Default.LastTabsCollection = new();

            MainViewModel = new MainViewModel(this);
            PopupViewModel = new PopupViewModel(this);
        }

        public void SaveSettings()
        {
            //Save opened tabs if OpenLastTabs is enable
            if (Properties.Settings.Default.OpenLastTabs)
            {
                Properties.Settings.Default.LastTabsCollection.Clear();
                foreach (var tab in MainViewModel.Tabs)
                    Properties.Settings.Default.LastTabsCollection.Add(tab.CurrentPath);
            }
            else Properties.Settings.Default.LastTabsCollection.Clear();

            //Save settings
            Properties.Settings.Default.Save();
        }

        public void ChangedWindowState(Rect restoreBounds)
        {
            Properties.Settings.Default.WindowWidth = restoreBounds.Width;
            Properties.Settings.Default.WindowHeight = restoreBounds.Height;
            Properties.Settings.Default.WindowLeft = restoreBounds.Left;
            Properties.Settings.Default.WindowTop = restoreBounds.Top;
        }
    }
}
