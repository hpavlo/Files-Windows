using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Interop;

namespace FileExplorer.Models
{
    public class DrivesModel
    {
        public ObservableCollection<FileDetailsModel> Drives { get; }

        public DrivesModel()
        {
            Drives = new ObservableCollection<FileDetailsModel>();

            LoadDrives();

            //Event to connect or remove a new disk
            HwndSource hwndSource = new HwndSource(0, 0, 0, 0, 0, "fake", IntPtr.Zero);
            if (hwndSource != null)  //Attaching to window procedure
                hwndSource.AddHook(new HwndSourceHook(HwndSourceHook));
        }

        private void LoadDrives()
        {
            Drives.Clear();
            foreach (var drive in DriveInfo.GetDrives())
            {
                string name = string.IsNullOrEmpty(drive.VolumeLabel) ? "Local Disk" : drive.VolumeLabel;
                Drives.Add(new FileDetailsModel
                {
                    Name = $"{name}({drive.Name.Replace("\\", "")})",
                    Path = drive.RootDirectory.FullName,
                    IsDirectory = true
                });
            }
        }

        //Method of handling a connection event or deleting a new disk
        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 537) //WM_DEVICECHANGE
            {
                if ((int)wParam == 0x8000 || //DBT_DEVICEARRIVAL
                    (int)wParam == 0x8004)   //DBT_DEVICEREMOVECOMPLETE
                    LoadDrives();
            }
            return IntPtr.Zero;
        }
    }
}
