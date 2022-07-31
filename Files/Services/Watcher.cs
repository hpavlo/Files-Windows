using Files.Interfaces;
using System.IO;
using System.Windows;

namespace Files.Services
{
    internal class Watcher
    {
        private const int delayWatcherEvent = 10;
        private FileSystemWatcher FileWatcher;

        private ICenterPanelViewModel CenterPanelVM;

        public Watcher(ICenterPanelViewModel centerPanelVM)
        {
            CenterPanelVM = centerPanelVM;

            try
            {
                FileWatcher = new FileSystemWatcher(CenterPanelVM.CurrentPath);
                FileWatcher.EnableRaisingEvents = true;
                FileWatcher.Created += WatcherEvent;
                FileWatcher.Deleted += WatcherEvent;
                FileWatcher.Renamed += WatcherEventModified;
                FileWatcher.Changed += WatcherEventModified;
            }
            catch { }
        }

        public void SetPath(string path)
        {
            try
            {
                FileWatcher.Path = path;
            }
            catch { }
        }

        /// <summary>
        /// Created/deleted file or folder event
        /// </summary>
        private void WatcherEvent(object sender, FileSystemEventArgs e)
        {
            System.Threading.Thread.Sleep(delayWatcherEvent);

            if (Application.Current == null) return;
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (!CenterPanelVM.SearchMode)
                    CenterPanelVM.UpdateFilesList(false);
            });
        }

        /// <summary>
        /// Rename/changed file or folder event
        /// </summary>
        private void WatcherEventModified(object sender, FileSystemEventArgs e)
        {
            System.Threading.Thread.Sleep(delayWatcherEvent);

            if (Application.Current == null) return;
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (e.ChangeType == WatcherChangeTypes.Renamed)
                {
                    for (int i = 0; i < CenterPanelVM.Files.Count; i++)
                    {
                        if (!File.Exists(CenterPanelVM.Files[i].FullName))
                        {
                            if (!Directory.Exists(CenterPanelVM.Files[i].FullName))
                            {
                                CenterPanelVM.Files[i] = GetFiles.GetFileDetails(e.FullPath);
                                CenterPanelVM.FilesChanged();
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < CenterPanelVM.Files.Count; i++)
                    {
                        if (CenterPanelVM.Files[i].FullName == e.FullPath)
                        {
                            if (!File.Exists(e.FullPath))
                                if (!Directory.Exists(e.FullPath))
                                    continue;
                            CenterPanelVM.Files[i] = GetFiles.GetFileDetails(e.FullPath);
                            CenterPanelVM.FilesChanged();
                        }
                    }
                }
            });
        }
    }
}
