using Files.Enums;
using Files.ViewModels.Base;
using Files.ViewModels.PopupViewModels;
using System;
using System.Windows;

namespace Files.ViewModels
{
    internal class PopupViewModel : BaseViewModel
    {
        //public MainWindowViewModel MainWindowViewModel;

        private RenamePopupViewModel _penamePopupViewModel;
        public RenamePopupViewModel RenamePopupViewModel
        {
            get => _penamePopupViewModel;
            set => Set(ref _penamePopupViewModel, value);
        }

        private AddDirectoryPopupViewModel _addDirectoryPopupViewModel;
        public AddDirectoryPopupViewModel AddDirectoryPopupViewModel
        {
            get => _addDirectoryPopupViewModel;
            set => Set(ref _addDirectoryPopupViewModel, value);
        }

        private SettingsPopupViewModel _settingsPopupViewModel;
        public SettingsPopupViewModel SettingsPopupViewModel
        {
            get => _settingsPopupViewModel;
            set => Set(ref _settingsPopupViewModel, value);
        }


        private PopupType _openDialogType = PopupType.None;
        public PopupType OpenDialogType
        {
            get => _openDialogType;
            set
            {
                if (_openDialogType != value)
                {
                    _openDialogType = value;
                    OnPropertyChanged();
                }
            }
        }

        public PopupViewModel(MainWindowViewModel mainWindowViewModel)
        {
            RenamePopupViewModel = new RenamePopupViewModel();
            AddDirectoryPopupViewModel = new AddDirectoryPopupViewModel();
            SettingsPopupViewModel = new SettingsPopupViewModel();
        }

        public void OpenRenamePopup(string oldFullName, Action<string?> callback)
        {
            RenamePopupViewModel.Init(oldFullName, result =>
            {
                OpenDialogType = PopupType.None;
                callback(result);
            });
            OpenDialogType = PopupType.Rename;
        }

        public void AddDirectoryPopup(Action<string?> callback)
        {
            AddDirectoryPopupViewModel.Init(result =>
            {
                OpenDialogType = PopupType.None;
                callback(result);
            });
            OpenDialogType = PopupType.AddDirectory;
        }

        public void OpenSettingsPopup(Action<bool> callback)
        {
            SettingsPopupViewModel.Init((isShowHiddenFilesChanges) =>
            {
                OpenDialogType = PopupType.None;
                callback(isShowHiddenFilesChanges);
            });
            OpenDialogType = PopupType.Settings;
        }
    }
}
