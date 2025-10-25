using System;
using System.Globalization;
using System.Windows.Data;

namespace vimage_settings
{
    public class EnumConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            // convert int to enum
            if (targetType.IsEnum)
                return Enum.ToObject(targetType, value);

            // convert enum to int
            return value.GetType().IsEnum
                ? System.Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()))
                : null;
        }

        public object? ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            // perform the same conversion in both directions
            return Convert(value, targetType, parameter, culture);
        }
    }
}
