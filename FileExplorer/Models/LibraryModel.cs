using Syroot.Windows.IO;
using System.Collections.ObjectModel;

namespace FileExplorer.Models
{
    public class LibraryModel
    {
        public ObservableCollection<FileDetailsModel> Library { get; }

        public LibraryModel()
        {
            Library = new ObservableCollection<FileDetailsModel>();

            LoadLibrary();
        }

        private void LoadLibrary()
        {
            KnownFolderType[] types = {
                KnownFolderType.Desktop,
                KnownFolderType.Documents,
                KnownFolderType.Downloads,
                KnownFolderType.Music,
                KnownFolderType.Pictures,
                KnownFolderType.Videos
            };

            foreach (var type in types)
            {
                Library.Add(new FileDetailsModel
                {
                    Name = type.ToString(),
                    IsDirectory = true,
                    Path = new KnownFolder(type).Path
                });
            }
        }
    }
}
