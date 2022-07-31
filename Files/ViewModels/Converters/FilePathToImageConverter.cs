using Files.Enums;
using Files.Services;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Files.ViewModels.Converters
{
    public class FilePathToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return GetFileIcon.GetIcon(value as string, FileIconSize.SmallIcon);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
