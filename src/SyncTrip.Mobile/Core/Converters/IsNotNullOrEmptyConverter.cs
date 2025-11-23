using System.Globalization;

namespace SyncTrip.Mobile.Core.Converters;

/// <summary>
/// Converter XAML pour vérifier si une chaîne n'est pas nulle ou vide.
/// Utilisé pour contrôler la visibilité d'éléments UI conditionnés par un message.
/// </summary>
public class IsNotNullOrEmptyConverter : IValueConverter
{
    /// <summary>
    /// Convertit une valeur string en booléen indiquant si elle n'est pas nulle ou vide.
    /// </summary>
    /// <param name="value">Valeur string à vérifier.</param>
    /// <param name="targetType">Type cible (non utilisé).</param>
    /// <param name="parameter">Paramètre (non utilisé).</param>
    /// <param name="culture">Culture (non utilisée).</param>
    /// <returns>True si la valeur n'est pas nulle ou vide, False sinon.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            return !string.IsNullOrEmpty(stringValue);
        }
        return value != null;
    }

    /// <summary>
    /// Conversion inverse non supportée.
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
