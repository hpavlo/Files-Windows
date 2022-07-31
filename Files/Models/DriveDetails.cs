using System.IO;

namespace Files.Models
{
    internal class DriveDetails
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public DriveType Type { get; set; }
        public bool IsSystemDrive { get; set; }
    }
}
