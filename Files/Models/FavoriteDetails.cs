using Files.Enums;

namespace Files.Models
{
    internal class FavoriteDetails
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public KnownFolder Folder { get; set; }
    }
}
