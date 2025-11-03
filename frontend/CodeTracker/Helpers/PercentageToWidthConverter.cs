using System;
using System.Globalization;
using System.Windows.Data;

namespace CodeTracker.Helpers
{
    public class PercentageToWidthConverter : IValueConverter
    {
        public double MaxWidth { get; set; } = 300;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double percentage)
            {
                return (percentage / 100.0) * MaxWidth;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
