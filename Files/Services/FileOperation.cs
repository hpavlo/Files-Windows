using Files.Enums;
using Files.Resources.MultilingualResources;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace Files.Services
{
    public static class FileOperation
    {
        private static byte[] cutClipboardBytes = { 2, 0, 0, 0 };
        private static byte[] copyClipboardBytes = { 5, 0, 0, 0 };

        private static class OperationParam
        {
            public static FileOperationType OperationType;
            public static string[] Files;
            public static string? Target;
        }

        private static Action<bool>? actionBgWorkerComplated;

        private static BackgroundWorker BgWorker;

        static FileOperation()
        {
            BgWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            BgWorker.DoWork += DoWork;
            BgWorker.ProgressChanged += ProgressChanged;
            BgWorker.RunWorkerCompleted += RunWorkerCompleted;
        }

        public static void BackgroundOperation(FileOperationType operationType, string[] files, string? target = null, Action<bool>? action = null)
        {
            OperationParam.OperationType = operationType;
            OperationParam.Files = files;
            OperationParam.Target = target;

            actionBgWorkerComplated = action;

            if (BgWorker.IsBusy)
                MessageDialog.Show(TranslationSource.Instance["FileOperationBgWorkerIsBusy"]);
            else
                BgWorker.RunWorkerAsync();
        }


        private static void DoWork(object? sender, DoWorkEventArgs e)
        {
            string? errorMessage = null;

            switch (OperationParam.OperationType)
            {
                case FileOperationType.Copy:
                    errorMessage = Copy(OperationParam.Files, OperationParam.Target);
                    break;
                case FileOperationType.Move:
                    errorMessage = Move(OperationParam.Files, OperationParam.Target);
                    break;
                case FileOperationType.Delete:
                    errorMessage = Delete(OperationParam.Files, false);
                    break;
                case FileOperationType.DeleteToRecycleBin:
                    errorMessage = Delete(OperationParam.Files, true);
                    break;
            }

            //Sleep for all animations complete
            System.Threading.Thread.Sleep(10);
            BgWorker.ReportProgress(0, errorMessage);
            e.Result = errorMessage;
        }

        private static void ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                if (actionBgWorkerComplated != null)
                    actionBgWorkerComplated(false);

                MessageDialog.Show(e.UserState.ToString(), TranslationSource.Instance["CaptionError"],
                    MessageDialogButton.OK, MessageDialogImage.Stop);
            }
        }

        private static void RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null && actionBgWorkerComplated != null)
                actionBgWorkerComplated(true);
        }

        private static string? Copy(string[] files, string target)
        {
            foreach (var file in files)
            {
                var arrName = file.Split('\\');

                var targetFile = target + "\\" + arrName[arrName.Length - 1];

                try
                {
                    if (Directory.Exists(file))
                    {
                        CreateDirectory(targetFile);
                        FileSystem.CopyDirectory(file, targetFile, UIOption.AllDialogs);
                    }
                    else FileSystem.CopyFile(file, targetFile, UIOption.AllDialogs);
                }
                catch (Exception e)
                {
                    return TranslationSource.Instance["FileOperationCan'tCopyElement"] + "\n\n" +
                        TranslationSource.Instance["FileOperationSourceElement:"] + file + "\n" +
                        TranslationSource.Instance["FileOperationTarget:"] + targetFile;
                }
            }

            return null;
        }

        private static string? Move(string[] files, string target)
        {
            foreach (var file in files)
            {
                var arrName = file.Split('\\');

                var targetFile = target + "\\" + arrName[arrName.Length - 1];

                try
                {
                    if (Directory.Exists(file))
                    {
                        CreateDirectory(targetFile);
                        FileSystem.MoveDirectory(file, targetFile, UIOption.AllDialogs);
                    }
                    else FileSystem.MoveFile(file, targetFile, UIOption.AllDialogs);
                }
                catch (Exception e)
                {
                    return TranslationSource.Instance["FileOperationCan'tMoveElement"] + "\n\n" +
                        TranslationSource.Instance["FileOperationSourceElement:"] + file + "\n" +
                        TranslationSource.Instance["FileOperationTarget:"] + targetFile;
                }
            }

            return null;
        }

        private static string? Delete(string[] files, bool toRecycleBin)
        {
            foreach (var file in files)
            {
                try
                {
                    if (Directory.Exists(file))
                        FileSystem.DeleteDirectory(file, UIOption.AllDialogs, toRecycleBin ?
                            RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently);
                    else
                        FileSystem.DeleteFile(file, UIOption.AllDialogs, toRecycleBin ? 
                            RecycleOption.SendToRecycleBin : RecycleOption.DeletePermanently);
                }
                catch (Exception e)
                {
                    return TranslationSource.Instance["FileOperationCan'tDeleteElement"] + "\n\n" +
                        TranslationSource.Instance["FileOperationSourceElement:"] + file;
                }
            }

            return null;
        }

        public static bool CreateDirectory(string fullName)
        {
            try
            {
                Directory.CreateDirectory(fullName);
            }
            catch
            {
                MessageDialog.Show(TranslationSource.Instance["FileOperationCan'tCreateDirectory"] + fullName,
                    TranslationSource.Instance["CaptionError"], MessageDialogButton.OK, MessageDialogImage.Stop);
                return false;
            }

            return true;
        }

        public static bool Rename(string oldFullName, string newName)
        {
            //If the new name equals the old name
            if (oldFullName.Split('\\')[^1] == newName)
                return false;

            try
            {
                if (Directory.Exists(oldFullName))
                    FileSystem.RenameDirectory(oldFullName, newName);
                else FileSystem.RenameFile(oldFullName, newName);
            }
            catch
            {
                MessageDialog.Show(TranslationSource.Instance["FileOperationCan'tRenameFile"] + oldFullName,
                    TranslationSource.Instance["CaptionError"], MessageDialogButton.OK, MessageDialogImage.Stop);
                return false;
            }

            return true;
        }

        public static bool CutToClipboard(StringCollection stringCollection)
        {
            try
            {
                MemoryStream dropEffect = new MemoryStream();
                dropEffect.Write(cutClipboardBytes, 0, cutClipboardBytes.Length);

                DataObject data = new DataObject();
                data.SetFileDropList(stringCollection);
                data.SetData("Preferred DropEffect", dropEffect);

                Clipboard.Clear();
                Clipboard.SetDataObject(data, true);
            }
            catch { return false; }

            return true;
        }

        public static bool CopyToClipboard(StringCollection stringCollection)
        {
            try
            {
                MemoryStream dropEffect = new MemoryStream();
                dropEffect.Write(copyClipboardBytes, 0, copyClipboardBytes.Length);

                DataObject data = new DataObject();
                data.SetFileDropList(stringCollection);
                data.SetData("Preferred DropEffect", dropEffect);

                Clipboard.Clear();
                Clipboard.SetDataObject(data, true);
            }
            catch { return false; }

            return true;
        }

        public static bool PasteFromClipboard(string target)
        {
            try
            {
                DataObject data = (DataObject)Clipboard.GetDataObject();
                if (data.ContainsFileDropList())
                {
                    StringCollection stringCollection = data.GetFileDropList();
                    string[] files = new string[stringCollection.Count];
                    stringCollection.CopyTo(files, 0);

                    MemoryStream dropEffect = (MemoryStream)data.GetData("Preferred DropEffect");
                    byte[] bytes = new byte[dropEffect.Length];

                    for (int i = 0; i < dropEffect.Length; i++)
                        bytes[i] = (byte)dropEffect.ReadByte();

                    if (Enumerable.SequenceEqual(bytes, cutClipboardBytes))
                        BackgroundOperation(FileOperationType.Move, files, target);
                    else if (Enumerable.SequenceEqual(bytes, copyClipboardBytes))
                        BackgroundOperation(FileOperationType.Copy, files, target);
                }
            }
            catch { return false; }

            return true;
        }
    }

    public enum FileOperationType
    {
        Copy,
        Move,
        Delete,
        DeleteToRecycleBin
    }
}
