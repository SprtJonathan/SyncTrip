using System.Globalization;

namespace SyncTrip.Mobile.Core.Converters;

/// <summary>
/// Converter XAML pour inverser une valeur booléenne.
/// Utilisé notamment pour désactiver un bouton pendant un chargement.
/// </summary>
public class InvertedBoolConverter : IValueConverter
{
    /// <summary>
    /// Convertit une valeur booléenne en son inverse.
    /// </summary>
    /// <param name="value">Valeur booléenne à convertir.</param>
    /// <param name="targetType">Type cible (non utilisé).</param>
    /// <param name="parameter">Paramètre (non utilisé).</param>
    /// <param name="culture">Culture (non utilisée).</param>
    /// <returns>Valeur inversée si le type est bool, sinon la valeur originale.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return value;
    }

    /// <summary>
    /// Convertit une valeur booléenne en son inverse (conversion inverse).
    /// </summary>
    /// <param name="value">Valeur booléenne à convertir.</param>
    /// <param name="targetType">Type cible (non utilisé).</param>
    /// <param name="parameter">Paramètre (non utilisé).</param>
    /// <param name="culture">Culture (non utilisée).</param>
    /// <returns>Valeur inversée si le type est bool, sinon la valeur originale.</returns>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return value;
    }
}
