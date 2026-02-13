using System.Globalization;
using Avalonia.Data.Converters;

namespace SyncTrip.App.Core.Converters;

public class ProposalStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int status)
        {
            return status switch
            {
                1 => "En cours",
                2 => "Accepte",
                3 => "Rejete",
                _ => "Inconnu"
            };
        }
        return "Inconnu";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
