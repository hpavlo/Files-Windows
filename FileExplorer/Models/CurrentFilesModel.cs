using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileExplorer.Models
{
    public class CurrentFilesModel
    {
        public ObservableCollection<FileDetailsModel> CurrentFiles { get; }

        private BackgroundWorker LoadFilesBackgroundWorker = new BackgroundWorker()
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true
        };

        public CurrentFilesModel(string startFolderPath)
        {
            CurrentFiles = new ObservableCollection<FileDetailsModel>();

            LoadFilesBackgroundWorker.DoWork += LoadFilesBackgroundWorker_DoWork;
            LoadFilesBackgroundWorker.ProgressChanged += LoadFilesBackgroundWorker_ProgressChanged;
            LoadFilesBackgroundWorker.RunWorkerCompleted += LoadFilesBackgroundWorker_RunWorkerCompleted;

            LoadFiles(startFolderPath);
        }

        public void LoadFiles(string path)
        {
            if (LoadFilesBackgroundWorker.IsBusy) return;
            if (!Directory.Exists(path))
            {
                CurrentFiles.Clear();
                MessageBox.Show("Folder not found!!!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                return;
            }

            CurrentFiles.Clear();
            GC.Collect();

            LoadFilesBackgroundWorker.CancelAsync();
            LoadFilesBackgroundWorker.RunWorkerAsync(path);
        }

        public bool BackgroundWorkerIsBusy => LoadFilesBackgroundWorker.IsBusy;

        private void LoadFilesBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string path = e.Argument as string;
            IEnumerable<string> listFilesCollection = new ReadOnlyCollectionBuilder<string>(FileSystem.GetDirectories(path)).Concat(FileSystem.GetFiles(path));
            ImageSource fileIcon;

            foreach (string filePath in listFilesCollection)
            {
                fileIcon = null;

                if (Directory.Exists(filePath))
                {
                    if ((FileSystem.GetDirectoryInfo(filePath).Attributes & FileAttributes.Hidden) != 0) continue;
                }
                else
                {
                    if ((FileSystem.GetFileInfo(filePath).Attributes & FileAttributes.Hidden) != 0) continue;

                    //fileIcon = Imaging.CreateBitmapSourceFromHIcon(Icon.ExtractAssociatedIcon(filePath)
                    //    .Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    fileIcon = Imaging.CreateBitmapSourceFromHIcon(IconReader.GetFileIcon(/*Path.GetExtension()*/filePath, IconReader.IconSize.Large, false)
                        .Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    fileIcon.Freeze();
                }

                (sender as BackgroundWorker).ReportProgress(1, new FileDetailsModel
                {
                    Name = Path.GetFileName(filePath),
                    Path = filePath,
                    IsDirectory = File.GetAttributes(filePath).HasFlag(FileAttributes.Directory),
                    FileIcon = fileIcon
                });
            }
        }

        private void LoadFilesBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) => CurrentFiles.Add(e.UserState as FileDetailsModel);

        private void LoadFilesBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { }

    }
}
