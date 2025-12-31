using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ToolCollectionManager.Converters
{
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrEmpty(str))
            {
                // If string is NOT empty (Has Path)
                // If parameter is "Inverse", we want to HIDE the placeholder.
                if (parameter?.ToString() == "Inverse")
                    return Visibility.Collapsed;
                // Otherwise, we want to SHOW the image.
                return Visibility.Visible; 
            }
            
            // If string IS empty (No Path)
            // If parameter is "Inverse", we want to SHOW the placeholder.
            if (parameter?.ToString() == "Inverse")
                return Visibility.Visible;
            // Otherwise, we want to HIDE the image.
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}