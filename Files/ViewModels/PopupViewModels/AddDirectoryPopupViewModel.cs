using Files.Interfaces;
using Files.Resources.MultilingualResources;
using Files.ViewModels.Base;
using Files.ViewModels.Commands;
using System;
using System.IO;
using System.Windows.Input;

namespace Files.ViewModels.PopupViewModels
{
    internal class AddDirectoryPopupViewModel : BaseViewModel, IPopupViewModel
    {
        private Action<string?> Callback;
        public bool ShowPopupInvalidName { get; set; } = false;

        private bool _isShowing = false;
        public bool IsShowing
        {
            get => _isShowing;
            set => Set(ref _isShowing, value);
        }

        private string _directoryName;
        public string DirectoryName
        {
            get => _directoryName;
            set => Set(ref _directoryName, value);
        }

        public void Init(Action<string?> callback)
        {
            IsShowing = true;
            Callback = callback;
            DirectoryName = TranslationSource.Instance["AddDirectoryPopupDefoultFoldersName"];
        }

        private bool CheckName()
        {
            var invalidNameChars = Path.GetInvalidFileNameChars();

            for (int i = 0; i < invalidNameChars.Length; i++)
            {
                if (DirectoryName.Contains(invalidNameChars[i]))
                {
                    ShowPopupInvalidName = true;
                    OnPropertyChanged(nameof(ShowPopupInvalidName));

                    return false;
                }
            }

            return true;
        }

        private ICommand _addDirectoryCommand;
        public ICommand AddDirectoyCommand => _addDirectoryCommand ??
                    (_addDirectoryCommand = new RelayCommand(obj =>
                    {
                        if (CheckName())
                        {
                            IsShowing = false;
                            Callback(DirectoryName);
                        }
                    }));

        private ICommand _discardCommand;
        public ICommand DiscardCommand => _discardCommand ??
                    (_discardCommand = new RelayCommand(obj =>
                    {
                        IsShowing = false;
                        Callback(null);
                    }));
    }
}
