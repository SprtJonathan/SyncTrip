using System.Globalization;
using Avalonia.Data.Converters;

namespace SyncTrip.App.Core.Converters;

public class IsNotNullOrEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
            return !string.IsNullOrEmpty(stringValue);
        return value != null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
