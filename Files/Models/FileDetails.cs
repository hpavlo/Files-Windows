using Files.Enums;
using Files.Resources.MultilingualResources;
using Files.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Files.Models
{
    public class FileDetails : INotifyPropertyChanged
    {
        private Dictionary<string, string> fileTypeList;

        public string Name { get; set; }
        public string FullName { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsHidden { get; set; }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (IsSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _allowDrop;
        public bool AllowDrop
        {
            get => _allowDrop;
            set
            {
                _allowDrop = value;
                OnPropertyChanged();
            }
        }
        public KnownFolder? Folder
        {
            get
            {
                if (!IsDirectory) return null;

                foreach (KnownFolder folder in Enum.GetValues(typeof(KnownFolder)))
                {
                    if (FullName == GetFavorites.GetPath(folder))
                        return folder;
                }

                return null;
            }
        }
        public ObservableCollection<IMenuItem> ContextMenu { get; set; }
        public string Type => IsDirectory ? TranslationSource.Instance["FileTypeFolder"] :
            fileTypeList.TryGetValue(Extension, out string type) ? type : TranslationSource.Instance["FileTypeFile"];

        public FileDetails()
        {
            fileTypeList = new Dictionary<string, string>();

            fileTypeList.Add(".txt", TranslationSource.Instance["FileTypeTextDocument"]);
            fileTypeList.Add(".docx", TranslationSource.Instance["FileTypeWordDocument"]);
            fileTypeList.Add(".xlsx", TranslationSource.Instance["FileTypeExcelDocument"]);
            fileTypeList.Add(".pptx", TranslationSource.Instance["FileTypePowerPointDocument"]);

            fileTypeList.Add(".png", TranslationSource.Instance["FileTypeImage"]);
            fileTypeList.Add(".jpg", TranslationSource.Instance["FileTypeImage"]);

            fileTypeList.Add(".mp4", TranslationSource.Instance["FileTypeVideo"]);
            fileTypeList.Add(".avi", TranslationSource.Instance["FileTypeVideo"]);

            fileTypeList.Add(".mp3", TranslationSource.Instance["FileTypeMusic"]);

            fileTypeList.Add(".exe", TranslationSource.Instance["FileTypeApplication"]);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
