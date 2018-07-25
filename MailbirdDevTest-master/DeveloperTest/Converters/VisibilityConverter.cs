using System;
using System.Globalization;
using System.Windows.Data;

namespace DeveloperTest.Converters
{
    class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            if (value is bool)
            {
                var v = System.Convert.ToBoolean(value);
                return v ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
