using Files.Models;
using Files.Resources.MultilingualResources;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Interop;

namespace Files.Services
{
    internal class GetDrives
    {
        private Action UpdateDrivesList;

        public GetDrives(Action updateDrivesList)
        {
            UpdateDrivesList = updateDrivesList;

            HwndSource hwndSource = new HwndSource(0, 0, 0, 0, 0, "DrivesUpdate", IntPtr.Zero);
            if (hwndSource != null)  //Attaching to window procedure
                hwndSource.AddHook(new HwndSourceHook(HwndSourceHook));
        }

        public void GetGrivesList(ObservableCollection<DriveDetails> drives)
        {
            drives.Clear();

            foreach (var drive in DriveInfo.GetDrives())
            {
                string name = string.IsNullOrEmpty(drive.VolumeLabel) ? TranslationSource.Instance["GetDrivesLocalDiskDefaultName"] : drive.VolumeLabel;
                drives.Add(new DriveDetails
                {
                    Name = $"{name} ({drive.Name.Replace("\\", "")})",
                    FullName = drive.RootDirectory.FullName,
                    Type = drive.DriveType,
                    IsSystemDrive = drive.Name.Contains(Environment.GetEnvironmentVariable("SystemDrive"))
                });
            }
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 537) //WM_DEVICECHANGE
            {
                if ((int)wParam == 0x8000 || //DBT_DEVICEARRIVAL
                    (int)wParam == 0x8004)   //DBT_DEVICEREMOVECOMPLETE
                    UpdateDrivesList();
            }

            return IntPtr.Zero;
        }
    }
}
