using Files.Enums;
using Files.Services;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Files.Models
{
    internal class TabButton : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _header;
        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsSystem));
                OnPropertyChanged(nameof(Folder));
                OnPropertyChanged(nameof(Drive));
            }
        }
        public int Index { get; set; }

        public bool IsSystem
        {
            get => Header.Contains(Environment.GetEnvironmentVariable("SystemDrive"));
        }

        public KnownFolder? Folder
        {
            get
            {
                foreach (KnownFolder folder in Enum.GetValues(typeof(KnownFolder)))
                {
                    if (FullName == GetFavorites.GetPath(folder))
                        return folder;
                }

                return null;
            }
        }

        public DriveType? Drive
        {
            get
            {
                if (new DirectoryInfo(FullName).Parent == null)
                    return new DriveInfo(FullName).DriveType;

                return null;
            }
        }

        public TabButton(string header, string fullName, int index)
        {
            Header = header;
            FullName = fullName;
            Index = index;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
