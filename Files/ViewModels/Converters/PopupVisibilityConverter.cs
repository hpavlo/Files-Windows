using Files.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Files.ViewModels.Converters
{
    internal class PopupVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (PopupType)value == PopupType.None ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
