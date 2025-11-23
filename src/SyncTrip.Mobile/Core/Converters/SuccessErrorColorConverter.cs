using System.Globalization;

namespace SyncTrip.Mobile.Core.Converters;

/// <summary>
/// Converter XAML pour convertir un booléen (succès/échec) en couleur appropriée.
/// Utilisé pour colorer les messages de feedback utilisateur.
/// </summary>
public class SuccessErrorColorConverter : IValueConverter
{
    /// <summary>
    /// Convertit une valeur booléenne en couleur (vert pour succès, rouge pour erreur).
    /// </summary>
    /// <param name="value">Valeur booléenne indiquant le succès (true) ou l'échec (false).</param>
    /// <param name="targetType">Type cible (non utilisé).</param>
    /// <param name="parameter">Paramètre (non utilisé).</param>
    /// <param name="culture">Culture (non utilisée).</param>
    /// <returns>Couleur verte si succès, rouge si échec.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSuccess)
        {
            return isSuccess ? Colors.Green : Colors.Red;
        }
        return Colors.Gray;
    }

    /// <summary>
    /// Conversion inverse non supportée.
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
