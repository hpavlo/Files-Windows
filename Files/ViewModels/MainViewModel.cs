using Files.ViewModels.Base;
using System;
using System.Collections.ObjectModel;

namespace Files.ViewModels
{
    internal class MainViewModel : BaseViewModel
    {
        internal MainWindowViewModel MainWindowViewModel;

        public TabContentViewModel SelectedTabContentViewModel => Tabs[SelectedTabIndex];

        public ObservableCollection<TabContentViewModel> Tabs { get; set; }
        public TabButtonsViewModel TabButtonsViewModel { get; set; }

        public string StartPath => Properties.Settings.Default.StartPath;

        private int _selectedTabIndex = 0;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (Set(ref _selectedTabIndex, value) && SelectedTabIndex >= 0)
                    MainWindowViewModel.SelectedTabChanged();
            }
        }

        public MainViewModel(MainWindowViewModel mainWindowViewModel)
        {
            if (Properties.Settings.Default.StartPath == string.Empty)
                Properties.Settings.Default.StartPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            MainWindowViewModel = mainWindowViewModel;

            TabButtonsViewModel = new TabButtonsViewModel(this);
            InitializeTabs();

            TabButtonsViewModel.UpdateTabButtonsList();
        }

        private void InitializeTabs()
        {
            Tabs = new ObservableCollection<TabContentViewModel>();

            //Read paths from the last opened tabs collection
            if (Properties.Settings.Default.OpenLastTabs &&
                Properties.Settings.Default.LastTabsCollection.Count > 0)
            {
                foreach (var tab in Properties.Settings.Default.LastTabsCollection)
                    Tabs.Add(new TabContentViewModel(tab, this));
            }
            else Tabs.Add(new TabContentViewModel(StartPath, this));
        }

        public void UpdateAllowDropTabs(string[] files, bool setpar)
        {
            foreach (var tab in Tabs)
                tab.CenterPanelViewModel.UpdateDragAllowItem(files, setpar);
        }

        public void UpdateFileListOnAllTabs()
        {
            foreach (var tab in Tabs)
                tab.CenterPanelViewModel.UpdateFilesList(false);
        }

        public void UpdateLanguageOnAllTabs()
        {
            TabButtonsViewModel.UpdateTabButtonsList(false);

            foreach (var tab in Tabs)
            {
                tab.LeftPanelViewModel.UpdateDrivesList();
                tab.LeftPanelViewModel.UpdateFavoritesList();
            }
        }
    }
}
