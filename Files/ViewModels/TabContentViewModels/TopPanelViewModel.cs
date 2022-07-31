using Files.Enums;
using Files.ViewModels.Base;
using Files.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Files.ViewModels.TabContentViewModels
{
    internal class TopPanelViewModel : BaseViewModel
    {
        internal TabContentViewModel TabContentViewModel;
        private PopupType PopupMode => TabContentViewModel.MainViewModel.MainWindowViewModel.PopupViewModel.OpenDialogType;

        public class PathListItem
        {
            public int Index { get; set; }
            public string Header { get; set; }
        }
        public ObservableCollection<PathListItem> PathList
        {
            get
            {
                var collection = new ObservableCollection<PathListItem>();
                var pathArr = Path.Split('\\');

                for (int i = 0; i < pathArr.Length; i++)
                {
                    if (pathArr[i] == string.Empty) continue;
                    collection.Add(new PathListItem()
                    {
                        Index = i,
                        Header = pathArr[i]
                    });
                }

                return collection;
            }
        }

        public void PathListChange() => OnPropertyChanged(nameof(PathList));

        public string Path
        {
            get => TabContentViewModel.CurrentPath;
            set
            {
                if (TabContentViewModel.CurrentPath == value || value == string.Empty)
                    return;

                if ((value[^1] == '\\' || value[^1] == '/') && !value.EndsWith(":\\"))
                    TabContentViewModel.CurrentPath = value.Remove(value.Length - 1);
                else if (value[^1] == ':')
                    TabContentViewModel.CurrentPath = value + '\\';
                else TabContentViewModel.CurrentPath = value;
            }
        }

        public TopPanelViewModel(TabContentViewModel tabContentViewModel)
        {
            TabContentViewModel = tabContentViewModel;
        }

        public void PathChanged() => OnPropertyChanged(nameof(Path));


        private ICommand _backCommand;
        public ICommand BackCommand => _backCommand ??
                    (_backCommand = new RelayCommand(obj =>
                    {
                        TabContentViewModel.CenterPanelViewModel.PreviousPath();
                    }, (obj) => PopupMode == PopupType.None));

        private ICommand _forwardCommand;
        public ICommand ForwardCommand => _forwardCommand ??
                    (_forwardCommand = new RelayCommand(obj =>
                    {
                        TabContentViewModel.CenterPanelViewModel.NextPath();
                    }, (obj) => PopupMode == PopupType.None));

        private ICommand _upCommand;
        public ICommand UpCommand => _upCommand ??
                    (_upCommand = new RelayCommand(obj =>
                    {
                        TabContentViewModel.CenterPanelViewModel.UpPath();
                    }, (obj) => PopupMode == PopupType.None));

        private ICommand _refreshCommand;
        public ICommand RefreshCommand => _refreshCommand ??
                    (_refreshCommand = new RelayCommand(obj =>
                    {
                        TabContentViewModel.CenterPanelViewModel.UpdateFilesList(true);
                    }, (obj) => PopupMode == PopupType.None));

        private ICommand _goToPathListItemCommand;
        public ICommand GoToPathListItemCommand => _goToPathListItemCommand ??
                    (_goToPathListItemCommand = new RelayCommand(obj =>
                    {
                        string toPath = string.Empty;
                        foreach (var item in PathList)
                        {
                            if (item.Index <= (int)obj)
                                toPath += item.Header + '\\';
                        }
                        Path = toPath;

                        TabContentViewModel.CenterPanelViewModel.UpdateFilesList(true);
                    }));

        private ICommand _openSettingsCommand;
        public ICommand OpenSettingsCommand => _openSettingsCommand ??
                    (_openSettingsCommand = new RelayCommand(obj =>
                    {
                        TabContentViewModel.MainViewModel.MainWindowViewModel.PopupViewModel.OpenSettingsPopup((isShowHiddenFilesChanges) =>
                        {
                            TabContentViewModel.MainViewModel.UpdateLanguageOnAllTabs();

                            if (isShowHiddenFilesChanges)
                                TabContentViewModel.MainViewModel.UpdateFileListOnAllTabs();
                        });
                    }));
    }
}

