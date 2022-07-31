using System.Windows;

namespace Files.Interfaces
{
    internal interface IPopupViewModel
    {
        /// <summary>
        /// Return true if we show a popup
        /// </summary>
        public bool IsShowing { get; set; }
    }
}
