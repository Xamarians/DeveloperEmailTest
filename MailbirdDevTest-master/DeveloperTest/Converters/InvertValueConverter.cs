using System;
using System.Globalization;
using System.Windows.Data;

namespace DeveloperTest.Converters
{
    class InvertValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            if (value is bool)
            {
                return !System.Convert.ToBoolean(value);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
