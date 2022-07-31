using Files.Enums;
using Files.Interfaces;
using Files.Models;
using Files.Resources.MultilingualResources;
using Files.Services;
using Files.ViewModels.Base;
using Files.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Files.ViewModels.TabContentViewModels
{
    internal class CenterPanelViewModel : BaseViewModel, ICenterPanelViewModel
    {
        internal TabContentViewModel TabContentViewModel;
        private PopupType PopupMode => TabContentViewModel.MainViewModel.MainWindowViewModel.PopupViewModel.OpenDialogType;

        private ListOfRecent<string> ListOfRecentPath;

        /// <summary>
        /// Path of directory to selecte after using ListOfRecentPath Previous
        /// </summary>
        public string? DirectoryToSelect { get; set; } = null;

        /// <summary>
        /// Should the folder be selected
        /// </summary>
        public bool SelectDirectory { get; set; } = false;

        /// <summary>
        /// Modified files or folders in current path watcher
        /// </summary>
        private Watcher FileWatcher;

        private bool _searchMode = false;
        public bool SearchMode
        {
            get => _searchMode;
            set
            {
                if (Set(ref _searchMode, value) && value == false)
                {
                    Search.Stop();
                    UpdateFilesList(true);
                }
            }
        }

        private bool _isSearching = false;
        public bool IsSearching
        {
            get => _isSearching;
            set => Set(ref _isSearching, value);
        }

        private double _progressBarMaxValue = 0;
        public double ProgressBarMaxValue
        {
            get => _progressBarMaxValue;
            set => Set(ref _progressBarMaxValue, value);
        }

        private double _progressBarValue = 0;
        public double ProgressBarValue
        {
            get => _progressBarValue;
            set => Set(ref _progressBarValue, value);
        }

        private bool _isSortUp = true;
        public bool IsSortUp
        {
            get => _isSortUp;
            set => Set(ref _isSortUp, value);
        }

        private ListHeader _sortType = ListHeader.Name;
        public ListHeader SortType
        {
            get => _sortType;
            set => Set(ref _sortType, value);
        }

        public ObservableCollection<FileDetails> Files { get; set; }

        public List<FileDetails> SelectedItems { get; set; } = new();

        private double _scrollPosition = 0;
        public double ScrollPosition
        {
            get => _scrollPosition;
            set => Set(ref _scrollPosition, value);
        }

        private int _foldersCount = 0;
        public int FoldersCount
        {
            get
            {
                if (SearchMode)
                    return _foldersCount;
                else
                {
                    try { return Directory.GetDirectories(CurrentPath).Count(path =>
                    (new FileInfo(path).Attributes & FileAttributes.Hidden) == 0 || Properties.Settings.Default.ShowHiddenFiles); }
                    catch { return _foldersCount; }
                }
            }
            private set => Set(ref _foldersCount, value);
        }

        private int _filesCount = 0;
        public int FilesCount
        {
            get
            {
                if (SearchMode)
                    return _filesCount;
                else
                {
                    try { return Directory.GetFiles(CurrentPath).Count(path =>
                    (new FileInfo(path).Attributes & FileAttributes.Hidden) == 0 || Properties.Settings.Default.ShowHiddenFiles); }
                    catch { return _filesCount; }
                }
            }
            private set => Set(ref _filesCount, value);
        }

        /// <summary>
        /// To show footer text 3 seconds
        /// </summary>
        private DispatcherTimer footerTextTimer = new DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 3)
        };

        private Action<bool> footerTextDoneAction;

        private string _footerText = string.Empty;
        public string FooterText
        {
            get => _footerText;
            set => Set(ref _footerText, value);
        }

        public string CurrentPath
        {
            get => TabContentViewModel.CurrentPath;
            set => TabContentViewModel.CurrentPath = value;
        }

        public CenterPanelViewModel(TabContentViewModel tabContentViewModel)
        {
            TabContentViewModel = tabContentViewModel;

            Files = new ObservableCollection<FileDetails>();
            ListOfRecentPath = new ListOfRecent<string>(CurrentPath);

            FileWatcher = new Watcher(this);

            UpdateFilesList(true);

            //Initialize footer text timer
            footerTextTimer.Tick += (s, e) =>
            {
                FooterText = string.Empty;
                footerTextTimer.Stop();
            };

            //If operation done to show footer text
            footerTextDoneAction = (success) =>
            {
                if (success)
                {
                    FooterText = TranslationSource.Instance["CenterPanelFooterTextDone"];
                    footerTextTimer.Start();
                }
                else FooterText = string.Empty;
            };
        }

        public void PathChanged()
        {
            OnPropertyChanged(nameof(CurrentPath));
            FoldersFilesCountChanged();

            ListOfRecentPath.Add(CurrentPath);
            UpdateFilesList(true);

            FileWatcher.SetPath(CurrentPath);
        }

        private void FoldersFilesCountChanged()
        {
            OnPropertyChanged(nameof(FoldersCount));
            OnPropertyChanged(nameof(FilesCount));
        }

        public void FilesChanged() => OnPropertyChanged(nameof(Files));

        public void PreviousPath()
        {
            if (SearchMode)
            {
                SearchMode = false;
                UpdateFilesList(true);
            }
            else
            {
                DirectoryToSelect = CurrentPath;
                SelectDirectory = true;
                CurrentPath = ListOfRecentPath.Previous();
            }
        }
        public void NextPath() => CurrentPath = ListOfRecentPath.Next();

        public void UpPath()
        {
            if (Directory.GetParent(CurrentPath) != null)
                CurrentPath = Directory.GetParent(CurrentPath).FullName;
        }

        public void UpdateFilesList(bool canCansel)
        {
            SearchMode = false;
            FoldersFilesCountChanged();
            GetFiles.GetFilesList(this, canCansel);
        }

        public void UpdateDragAllowItem(string[] files, bool setpar)
        {
            foreach (var file in files)
            {
                foreach (var curFile in Files)
                {
                    if (curFile.FullName == file)
                        curFile.AllowDrop = setpar;
                }
            }
        }

        public void UnselectItem()
        {
            foreach (var file in Files)
            {
                if (file.IsSelected)
                    file.IsSelected = false;
            }
        }

        public void SortList()
        {
            switch (SortType)
            {
                case ListHeader.Name:
                    if (IsSortUp)
                        Files = new(Files.OrderBy(f => f.Name));
                    else Files = new(Files.OrderByDescending(f => f.Name));
                    break;
                case ListHeader.DateModified:
                    if (IsSortUp)
                        Files = new(Files.OrderBy(f => f.ModifiedOn));
                    else Files = new(Files.OrderByDescending(f => f.ModifiedOn));
                    break;
                case ListHeader.Type:
                    if (IsSortUp)
                        Files = new(Files.OrderBy(f => f.Type));
                    else Files = new(Files.OrderByDescending(f => f.Type));
                    break;
                case ListHeader.Size:
                    if (IsSortUp)
                        Files = new(Files.OrderBy(f => f.Size));
                    else Files = new(Files.OrderByDescending(f => f.Size));
                    break;
            }

            if (SortType != ListHeader.Size)
                Files = new(Files.OrderByDescending(f => f.IsDirectory));

            FilesChanged();
        }
        public void OpenFile(FileDetails file)
        {
            if (file.IsDirectory)
            {
                if (SearchMode) SearchMode = false;

                CurrentPath = file.FullName;
            }

            else
            {
                try
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo(file.FullName)
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }
                catch
                {
                    MessageDialog.Show(TranslationSource.Instance["CenterPanelCan'tOpenFile"] + file,
                        TranslationSource.Instance["CaptionError"], MessageDialogButton.OK, MessageDialogImage.Stop);
                }
            }
        }

        public void MoveFiles(string[] files, string target)
        {
            FooterText = TranslationSource.Instance["CenterPanelFooterTextMove"];

            FileOperation.BackgroundOperation(FileOperationType.Move, files, target, footerTextDoneAction);
        }

        public void CopyFiles(string[] files, string target)
        {
            FooterText = TranslationSource.Instance["CenterPanelFooterTextCopy"];

            FileOperation.BackgroundOperation(FileOperationType.Copy, files, target, footerTextDoneAction);
        }

        public void CutFiles(string[] files)
        {
            var stringCollection = new StringCollection();
            stringCollection.AddRange(files);
            FileOperation.CutToClipboard(stringCollection);
        }

        public void CopyFilesToClipboard(string[] files)
        {
            var stringCollection = new StringCollection();
            stringCollection.AddRange(files);
            FileOperation.CopyToClipboard(stringCollection);
        }

        public void PasteFiles(string path)
        {
            FileOperation.PasteFromClipboard(path);
        }

        public void CreateDirectory(string name)
        {
            string fullName = CurrentPath + "\\" + name;

            FileOperation.CreateDirectory(fullName);
        }

        public void RenameFile(string oldFullName, string newName)
        {
            FileOperation.Rename(oldFullName, newName);
        }

        public void DeleteFiles(string[] files, bool toRecycleBin)
        {
            FooterText = TranslationSource.Instance["CenterPanelFooterTextDeleting"];

            if (toRecycleBin)
                FileOperation.BackgroundOperation(FileOperationType.DeleteToRecycleBin, files, null, footerTextDoneAction);
            else
                FileOperation.BackgroundOperation(FileOperationType.Delete, files, null, footerTextDoneAction);
        }

        public void SearchFiles(string searchPattern)
        {
            if (string.IsNullOrEmpty(searchPattern)) return;

            Files.Clear();

            FoldersCount = 0;
            FilesCount = 0;
            FoldersFilesCountChanged();

            IsSearching = true;
            Search.AsyncSearch(CurrentPath, searchPattern, file =>
            {
                if (file == null)
                {
                    //The search is over
                    FoldersCount = 0;
                    FilesCount = 0;
                    IsSearching = false;
                    return;
                }

                Files.Add(file);

                if (file.IsDirectory) FoldersCount++;
                else FilesCount++;
            });
        }


        private ICommand _sortListCommand;
        public ICommand SortListCommand => _sortListCommand ??
                    (_sortListCommand = new RelayCommand(obj =>
                    {
                        ListHeader clickType = (ListHeader)obj;
                        if (clickType == SortType)
                            IsSortUp = !IsSortUp;
                        else
                        {
                            IsSortUp = true;
                            SortType = clickType;
                        }

                        SortList();
                    }));

        private ICommand _openFileCommand;
        public ICommand OpenFileCommand => _openFileCommand ??
                    (_openFileCommand = new RelayCommand(obj =>
                    {
                        if (obj == null) return;
                        OpenFile((FileDetails)obj);
                    }));

        private ICommand _addDirectoryCommand;
        public ICommand AddDirectoryCommand => _addDirectoryCommand ??
                    (_addDirectoryCommand = new RelayCommand(obj =>
                    {
                        TabContentViewModel.MainViewModel.MainWindowViewModel.PopupViewModel.AddDirectoryPopup(directoryName =>
                        {
                            if (directoryName != null)
                                CreateDirectory(directoryName);
                        });
                    }, (obj) => !SearchMode && PopupMode == PopupType.None));

        private ICommand _cutFilesCommand;
        public ICommand CutFilesCommand => _cutFilesCommand ??
                    (_cutFilesCommand = new RelayCommand(obj =>
                    {
                        CutFiles(SelectedItems.ConvertAll(new Converter<FileDetails, string>(file => file.FullName)).ToArray());
                    }, (obj) => SelectedItems.Count > 0 && PopupMode == PopupType.None));

        private ICommand _copyFilesCommand;
        public ICommand CopyFilesCommand => _copyFilesCommand ??
                    (_copyFilesCommand = new RelayCommand(obj =>
                    {
                        CopyFilesToClipboard(SelectedItems.ConvertAll(new Converter<FileDetails, string>(file => file.FullName)).ToArray());
                    }, (obj) => SelectedItems.Count > 0 && PopupMode == PopupType.None));

        private ICommand _pasteFilesCommand;
        public ICommand PasteFilesCommand => _pasteFilesCommand ??
                    (_pasteFilesCommand = new RelayCommand(obj =>
                    {
                        if (SelectedItems.Count == 1 && SelectedItems[0].IsDirectory)
                            PasteFiles(SelectedItems[0].FullName);
                        else PasteFiles(CurrentPath);
                    }, (obj) => !SearchMode && PopupMode == PopupType.None));

        private ICommand _renameFileCommand;
        public ICommand RenameFileCommand => _renameFileCommand ??
                    (_renameFileCommand = new RelayCommand(obj =>
                    {
                        string oldFullName = SelectedItems[0].FullName;

                        TabContentViewModel.MainViewModel.MainWindowViewModel.PopupViewModel.OpenRenamePopup(oldFullName, newName =>
                        {
                            if (newName != null)
                            {
                                RenameFile(oldFullName, newName);
                            }
                        });
                    }, (obj) => SelectedItems.Count == 1 && PopupMode == PopupType.None));

        private ICommand _deleteFilesCommand;
        public ICommand DeleteFilesCommand => _deleteFilesCommand ??
                    (_deleteFilesCommand = new RelayCommand(obj =>
                    {
                        DeleteFiles(SelectedItems.ConvertAll(new Converter<FileDetails, string>(file => file.FullName)).ToArray(), (bool)obj);
                    }, (obj) => 
                    SelectedItems.Count > 0 && PopupMode == PopupType.None));

        private ICommand _searchFilesCommand;
        public ICommand SearchFilesCommand => _searchFilesCommand ??
                    (_searchFilesCommand = new RelayCommand(obj =>
                    {
                        if (!SearchMode)
                        {
                            SearchMode = true;
                            return;
                        }

                        if (string.IsNullOrEmpty((string)obj)) return;

                        SearchFiles((string)obj);
                    }, (obj) => PopupMode == PopupType.None));

        private ICommand _closeSearchCommand;
        public ICommand CloseSearchCommand => _closeSearchCommand ??
                    (_closeSearchCommand = new RelayCommand(obj =>
                    {
                        PreviousPath();
                    }));
    }
}
