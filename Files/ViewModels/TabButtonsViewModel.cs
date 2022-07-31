using Files.Enums;
using Files.Models;
using Files.ViewModels.Base;
using Files.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

namespace Files.ViewModels
{
    internal class TabButtonsViewModel : BaseViewModel
    {
        internal MainViewModel MainViewModel;
        private PopupType PopupMode => MainViewModel.MainWindowViewModel.PopupViewModel.OpenDialogType;
        public ObservableCollection<TabButton> TabButtons { get; set; }
        public int SelectedTabIndex
        {
            get => MainViewModel.SelectedTabIndex;
            set
            {
                MainViewModel.SelectedTabIndex = value;
                OnPropertyChanged();
            }
        }

        public double ScrollPosition => MainViewModel.SelectedTabIndex;

        public TabButtonsViewModel(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;

            TabButtons = new ObservableCollection<TabButton>();
        }

        public void UpdateHeader()
        {
            if (TabButtons.Count > 0 && SelectedTabIndex > -1)
            {
                TabButtons[SelectedTabIndex].Header = MainViewModel.Tabs[SelectedTabIndex].Header;
                TabButtons[SelectedTabIndex].FullName = MainViewModel.Tabs[SelectedTabIndex].CurrentPath;
            }
        }

        public void UpdateTabButtonsList(bool goToLastTab = true)
        {
            int selectTab = SelectedTabIndex;
            TabButtons.Clear();
            int i = 0;

            foreach (var item in MainViewModel.Tabs)
            {
                TabButtons.Add(new TabButton(item.Header, item.CurrentPath, i++));
            }

            OnPropertyChanged(nameof(TabButtons));

            SelectedTabIndex = goToLastTab ? TabButtons.Count - 1 : selectTab;
            OnPropertyChanged(nameof(ScrollPosition));
        }

        public void AddTab(string[] tabPaths, bool goToLastTab)
        {
            foreach (var path in tabPaths)
            {
                if (!Directory.Exists(path)) return;

                MainViewModel.Tabs.Add(new TabContentViewModel(path, MainViewModel));
            }

            UpdateTabButtonsList(goToLastTab);
        }

        public void RemoveTab(int index)
        {
            int toSelect = SelectedTabIndex;

            if ((toSelect == index && toSelect == TabButtons.Count - 1) ||
                index < toSelect) toSelect--;

            TabButtons.RemoveAt(index);
            MainViewModel.Tabs.RemoveAt(index);
            UpdateTabButtonsList();

            SelectedTabIndex = toSelect;
            if (toSelect < 0)
                MainViewModel.MainWindowViewModel.CloseWindow();
        }



        private ICommand _addTabCommand;
        public ICommand AddTabCommand => _addTabCommand ??
                    (_addTabCommand = new RelayCommand(obj =>
                    {
                        AddTab(new string[] { MainViewModel.StartPath }, true);
                    }, (obj) => PopupMode == PopupType.None));

        private ICommand _removeTabCommand;
        public ICommand RemoveTabCommand => _removeTabCommand ??
                    (_removeTabCommand = new RelayCommand(obj => RemoveTab((int)obj)));
    }
}
