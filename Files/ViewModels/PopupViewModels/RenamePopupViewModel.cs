using Files.Interfaces;
using Files.ViewModels.Base;
using Files.ViewModels.Commands;
using System;
using System.IO;
using System.Windows.Input;

namespace Files.ViewModels.PopupViewModels
{
    internal class RenamePopupViewModel : BaseViewModel, IPopupViewModel
    {
        private Action<string?> Callback;

        private bool _isShowing = false;
        public bool IsShowing
        {
            get => _isShowing;
            set => Set(ref _isShowing, value);
        }
        public bool ShowPopupInvalidName { get; set; } = false;

        private bool _isDirectoryRename = false;
        /// <summary>
        /// Open only text area to rename folder
        /// </summary>
        public bool IsDirectoryRename
        {
            get => _isDirectoryRename;
            private set => Set(ref _isDirectoryRename, value);
        }

        private bool _isFileRename = false;
        /// <summary>
        /// Open text areas to rename file and change extension
        /// </summary>
        public bool IsFileRename
        {
            get => _isFileRename;
            private set => Set(ref _isFileRename, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _extension;
        public string Extension
        {
            get => _extension;
            set => Set(ref _extension, value);
        }

        /// <summary>
        /// RenamePopupViewModel constructor
        /// </summary>
        /// <param name="oldFullName">Old name of file or directory</param>
        /// <param name="callback">Action with result</param>
        public void Init(string oldFullName, Action<string?> callback)
        {
            IsShowing = true;
            Callback = callback;

            IsDirectoryRename = Directory.Exists(oldFullName);
            IsFileRename = !IsDirectoryRename;

            if (IsFileRename)
            {
                //Get only file name without extension
                Name = Path.GetFileNameWithoutExtension(oldFullName);

                //Get file extension without dot
                Extension = Path.GetExtension(oldFullName).Remove(0, 1);
            }
            else
            {
                //Get directory name
                Name = new DirectoryInfo(oldFullName).Name;
            }
        }

        private bool CheckName()
        {
            var invalidNameChars = Path.GetInvalidFileNameChars();

            for (int i = 0; i < invalidNameChars.Length; i++)
            {
                if (Name.Contains(invalidNameChars[i]) ||
                    (Extension != null && Extension.Contains(invalidNameChars[i])))
                {
                    ShowPopupInvalidName = true;
                    OnPropertyChanged(nameof(ShowPopupInvalidName));

                    return false;
                }
            }

            return true;
        }

        private ICommand _renameCommand;
        public ICommand RenameCommand => _renameCommand ??
                    (_renameCommand = new RelayCommand(obj =>
                    {
                        if (CheckName())
                        {
                            IsShowing = false;
                            string newName = IsFileRename ? Name + '.' + Extension : Name;
                            Callback(newName);
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
