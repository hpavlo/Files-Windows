using Files.Enums;
using Files.Interfaces;
using Files.Models;
using Files.Resources.MultilingualResources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Files.Services
{
    public static class GetFiles
    {
        /// <summary>
        /// BackgroundWorker reading files and directories details
        /// </summary>
        private static BackgroundWorker BgWorker;
        /// <summary>
        /// Queue of ICenterPanelViewModels
        /// </summary>
        private static Queue<ICenterPanelViewModel> queueCenterPanelVM;
        /// <summary>
        /// Position of the selected item
        /// </summary>
        private static int? ScrollPosition = null;

        /// <summary>
        /// The first initialization of objects
        /// </summary>
        static GetFiles()
        {
            BgWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            BgWorker.DoWork += DoWork;
            BgWorker.ProgressChanged += ProgressChanged;
            BgWorker.RunWorkerCompleted += RunWorkerCompleted;

            queueCenterPanelVM = new Queue<ICenterPanelViewModel>();
        }


        /// <summary>
        /// Get the list of files
        /// </summary>
        /// <param name="centerPanelVM">CenterPanelViewModel with required information</param>
        /// <param name="canCansel">Can BackgroundWorker cansel the work</param>
        public static void GetFilesList(ICenterPanelViewModel centerPanelVM, bool canCansel)
        {
            queueCenterPanelVM.Enqueue(centerPanelVM);

            centerPanelVM.ProgressBarValue = 0;
            centerPanelVM.ProgressBarMaxValue = centerPanelVM.FilesCount + centerPanelVM.FoldersCount;

            if (!BgWorker.IsBusy)
                BgWorker.RunWorkerAsync();
            else if (canCansel)
                BgWorker.CancelAsync();
        }

        private static void DoWork(object? sender, DoWorkEventArgs e)
        {
            e.Result = new List<FileDetails>();

            try
            {
                Directory.GetDirectories(queueCenterPanelVM.Peek().CurrentPath).
                Concat(Directory.GetFiles(queueCenterPanelVM.Peek().CurrentPath)).
                ToList().ForEach(filePath =>
                {
                    if (BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var file = GetFileDetails(filePath);
                    if (Properties.Settings.Default.ShowHiddenFiles || !file.IsHidden)
                    {
                        ((List<FileDetails>)e.Result).Add(file);
                        if (filePath == queueCenterPanelVM.Peek().DirectoryToSelect &&
                            queueCenterPanelVM.Peek().SelectDirectory)
                        {
                            file.IsSelected = true;
                            ScrollPosition = ((List<FileDetails>)e.Result).IndexOf(file);
                        }
                        BgWorker.ReportProgress(1);
                    }
                });
            }
            catch (Exception exeption)
            {
                BgWorker.ReportProgress(0, exeption.Message);
            }
        }

        private static void ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.UserState == null)
                queueCenterPanelVM.Peek().ProgressBarValue += e.ProgressPercentage;
            else MessageDialog.Show((string)e.UserState, TranslationSource.Instance["CaptionError"],
                MessageDialogButton.OK, MessageDialogImage.Stop);
        }

        private static void RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            queueCenterPanelVM.Peek().SelectDirectory = false;

            if (e.Cancelled)
            {
                queueCenterPanelVM.Dequeue().ProgressBarValue = 0;
                BgWorker.RunWorkerAsync();
                return;
            }

            queueCenterPanelVM.Peek().Files.Clear();

            foreach (var fileDetails in (List<FileDetails>)e.Result)
                queueCenterPanelVM.Peek().Files.Add(fileDetails);

            queueCenterPanelVM.Peek().FilesChanged();
            queueCenterPanelVM.Peek().ProgressBarValue = 0;

            //Select directory and scroll to it
            queueCenterPanelVM.Peek().ScrollPosition = ScrollPosition ?? 0;
            ScrollPosition = null;

            queueCenterPanelVM.Dequeue();

            if (queueCenterPanelVM.Count > 0)
                BgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Get file or directory details
        /// </summary>
        /// <param name="filePath">Full path of file or directory</param>
        /// <returns></returns>
        public static FileDetails GetFileDetails(string filePath)
        {
            var fileDetails = new FileDetails();
            FileSystemInfo fileSystemInfo;

            if (Directory.Exists(filePath))
                fileSystemInfo = new DirectoryInfo(filePath);
            else fileSystemInfo = new FileInfo(filePath);

            //IsSelected property
            fileDetails.IsSelected = false;

            //IsDirectory property
            if (Directory.Exists(filePath))
                fileDetails.IsDirectory = true;
            else fileDetails.IsDirectory = false;

            //AllowDrop property
            fileDetails.AllowDrop = fileDetails.IsDirectory;

            //Name property
            fileDetails.Name = fileSystemInfo.Name;

            //FullName property
            fileDetails.FullName = fileSystemInfo.FullName;

            //Extension property
            fileDetails.Extension = !fileDetails.IsDirectory ? fileSystemInfo.Extension : null;

            //Size property
            if (!fileDetails.IsDirectory)
                fileDetails.Size = new FileInfo(filePath).Length;

            //CreatedOn property
            fileDetails.CreatedOn = File.GetCreationTime(filePath);

            //ModifiedOn property
            fileDetails.ModifiedOn = File.GetLastWriteTime(filePath);

            //IsHidden property
            if ((new FileInfo(filePath).Attributes & FileAttributes.Hidden) != 0)
                fileDetails.IsHidden = true;
            else fileDetails.IsHidden = false;

            //IsReadOnly property
            if ((new FileInfo(filePath).Attributes & FileAttributes.ReadOnly) != 0)
                fileDetails.IsReadOnly = true;
            else fileDetails.IsReadOnly = false;

            return fileDetails;
        }
    }
}
