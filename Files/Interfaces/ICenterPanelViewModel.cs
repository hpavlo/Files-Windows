using Files.Models;
using System.Collections.ObjectModel;

namespace Files.Interfaces
{
    public interface ICenterPanelViewModel
    {
        public string? DirectoryToSelect { get; set; }
        public bool SelectDirectory { get; set; }
        public bool SearchMode { get; set; }
        public ObservableCollection<FileDetails> Files { get; set; }
        public double ScrollPosition { get; set; }
        public double ProgressBarMaxValue { get; set; }
        public double ProgressBarValue { get; set; }
        public string CurrentPath { get; set; }
        public int FoldersCount { get; }
        public int FilesCount { get; }

        public void FilesChanged();
        public void UpdateFilesList(bool canCansel);
    }
}
