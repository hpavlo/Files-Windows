using FileExplorer.Models;
using FileExplorer.ViewModels.Commands;
using Syroot.Windows.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace FileExplorer.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Properties

        //INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private string startFolderPath = new KnownFolder(KnownFolderType.Desktop).Path;
        private FolderHistoryModel folderHistory;

        private string _currentPath;
        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                OnPropertyChanged();
            }
        }

        private DrivesModel drivesCollection;
        public ObservableCollection<FileDetailsModel> ConnectedDrives => drivesCollection.Drives;

        private LibraryModel libraryCollection;
        public ObservableCollection<FileDetailsModel> LibraryFolders => libraryCollection.Library;

        private CurrentFilesModel currentFilesCollection;
        public ObservableCollection<FileDetailsModel> CurrentFiles => currentFilesCollection.CurrentFiles;

        #endregion

        #region Constructor

        public MainViewModel()
        {
            CurrentPath = startFolderPath;
            folderHistory = new FolderHistoryModel(startFolderPath);

            drivesCollection = new DrivesModel();
            libraryCollection = new LibraryModel();
            currentFilesCollection = new CurrentFilesModel(CurrentPath);
        }

        #endregion

        #region Commands

        private ICommand _goToPathCommand;
        public ICommand GoToPathCommand
        {
            get
            {
                return _goToPathCommand ??
                    (_goToPathCommand = new RelayCommand(path =>
                    {
                        if (currentFilesCollection.BackgroundWorkerIsBusy) return;
                        if (path as string == null) return;

                        CurrentPath = path as string;
                        folderHistory.Add(CurrentPath);
                        currentFilesCollection.LoadFiles(CurrentPath);
                    }));
            }
        }

        private ICommand _goToFolderContentCommand;
        public ICommand GoToFolderContentCommand
        {
            get
            {
                return _goToFolderContentCommand ??
                    (_goToFolderContentCommand = new RelayCommand(file =>
                    {
                        var selectedFile = file as FileDetailsModel;
                        if (selectedFile == null) return;
                        if (selectedFile.IsDirectory)
                        {
                            if (currentFilesCollection.BackgroundWorkerIsBusy) return;

                            CurrentPath = selectedFile.Path;
                            folderHistory.Add(CurrentPath);
                            currentFilesCollection.LoadFiles(CurrentPath);
                        } 
                        else
                        {
                            new Process
                            {
                                StartInfo = new ProcessStartInfo(selectedFile.Path)
                                {
                                    UseShellExecute = true
                                }
                            }.Start();
                        }
                    }));
            }
        }

        private ICommand _goToPreviousFolderCommand;
        public ICommand GoToPreviousFolderCommand
        {
            get
            {
                return _goToPreviousFolderCommand ??
                    (_goToPreviousFolderCommand = new RelayCommand(obj => PreviousFolder()));
            }
        }

        private ICommand _goToNextFolderCommand;
        public ICommand GoToNextFolderCommand
        {
            get
            {
                return _goToNextFolderCommand ??
                    (_goToNextFolderCommand = new RelayCommand(obj => NextFolder()));
            }
        }

        private ICommand _goToParentFolderCommand;
        public ICommand GoToParentFolderCommand
        {
            get
            {
                return _goToParentFolderCommand ??
                    (_goToParentFolderCommand = new RelayCommand(obj =>
                    {
                        if (currentFilesCollection.BackgroundWorkerIsBusy) return;
                        if (CurrentPath.Length <= 3) return;

                        CurrentPath = Directory.GetParent(CurrentPath).FullName;
                        folderHistory.Add(CurrentPath);
                        currentFilesCollection.LoadFiles(CurrentPath);
                    }));
            }
        }

        private ICommand _refreshContentCommand;
        public ICommand RefreshContentCommand
        {
            get
            {
                return _refreshContentCommand ??
                    (_refreshContentCommand = new RelayCommand(obj =>
                    {
                        CurrentPath = folderHistory.CurrentFolder();
                        currentFilesCollection.LoadFiles(CurrentPath);
                    }));
            }
        }

        #endregion

        #region Methods

        public void PreviousFolder()
        {
            if (currentFilesCollection.BackgroundWorkerIsBusy) return;
            var tempPath = folderHistory.PreviousFolder();
            if (tempPath == CurrentPath) return;
            CurrentPath = tempPath;
            currentFilesCollection.LoadFiles(CurrentPath);
        }
        public void NextFolder()
        {
            if (currentFilesCollection.BackgroundWorkerIsBusy) return;
            var tempPath = folderHistory.NextFolder();
            if (tempPath == CurrentPath) return;
            CurrentPath = tempPath;
            currentFilesCollection.LoadFiles(CurrentPath);
        }

        //INotifyPropertyChanged
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
