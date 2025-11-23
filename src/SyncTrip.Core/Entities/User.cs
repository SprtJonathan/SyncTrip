using SyncTrip.Core.Exceptions;

namespace SyncTrip.Core.Entities;

/// <summary>
/// Représente un utilisateur de l'application SyncTrip.
/// </summary>
public class User
{
    /// <summary>
    /// Identifiant unique de l'utilisateur.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Adresse email de l'utilisateur (unique).
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Nom d'utilisateur (pseudo) - obligatoire.
    /// </summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>
    /// Prénom de l'utilisateur (facultatif).
    /// </summary>
    public string? FirstName { get; private set; }

    /// <summary>
    /// Nom de famille de l'utilisateur (facultatif).
    /// </summary>
    public string? LastName { get; private set; }

    /// <summary>
    /// Date de naissance de l'utilisateur (obligatoire).
    /// Doit avoir plus de 14 ans.
    /// </summary>
    public DateOnly BirthDate { get; private set; }

    /// <summary>
    /// URL de l'avatar de l'utilisateur (facultatif).
    /// </summary>
    public string? AvatarUrl { get; private set; }

    /// <summary>
    /// Date de création du compte.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date de dernière mise à jour du profil.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Indique si le compte est actif.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Date de désactivation du compte (si applicable).
    /// </summary>
    public DateTime? DeactivationDate { get; private set; }

    /// <summary>
    /// Âge minimum requis pour utiliser l'application.
    /// </summary>
    private const int MinimumAge = 14;

    // Constructeur privé pour EF Core
    private User()
    {
    }

    /// <summary>
    /// Factory method pour créer un nouvel utilisateur.
    /// </summary>
    /// <param name="email">Adresse email de l'utilisateur.</param>
    /// <param name="username">Nom d'utilisateur (pseudo).</param>
    /// <param name="birthDate">Date de naissance.</param>
    /// <param name="firstName">Prénom (facultatif).</param>
    /// <param name="lastName">Nom de famille (facultatif).</param>
    /// <returns>Nouvelle instance de User.</returns>
    /// <exception cref="DomainException">Si l'âge est inférieur ou égal à 14 ans.</exception>
    public static User Create(
        string email,
        string username,
        DateOnly birthDate,
        string? firstName = null,
        string? lastName = null)
    {
        ValidateAge(birthDate);

        var now = DateTime.UtcNow;

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            CreatedAt = now,
            UpdatedAt = now,
            IsActive = true
        };
    }

    /// <summary>
    /// Définit une nouvelle date de naissance avec validation.
    /// </summary>
    /// <param name="birthDate">Nouvelle date de naissance.</param>
    /// <exception cref="DomainException">Si l'âge est inférieur ou égal à 14 ans.</exception>
    public void SetBirthDate(DateOnly birthDate)
    {
        ValidateAge(birthDate);
        BirthDate = birthDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Met à jour les informations du profil.
    /// </summary>
    public void UpdateProfile(string? username, string? firstName, string? lastName, string? avatarUrl)
    {
        if (!string.IsNullOrWhiteSpace(username))
            Username = username;

        FirstName = firstName;
        LastName = lastName;
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Désactive le compte utilisateur.
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        DeactivationDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Réactive le compte utilisateur.
    /// </summary>
    public void Reactivate()
    {
        IsActive = true;
        DeactivationDate = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calcule l'âge actuel de l'utilisateur.
    /// </summary>
    /// <returns>Âge en années.</returns>
    public int CalculateAge()
    {
        return CalculateAge(BirthDate);
    }

    /// <summary>
    /// Calcule l'âge pour une date de naissance donnée.
    /// </summary>
    /// <param name="birthDate">Date de naissance.</param>
    /// <returns>Âge en années.</returns>
    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int age = today.Year - birthDate.Year;

        // Ajuster si l'anniversaire n'est pas encore passé cette année
        if (birthDate > today.AddYears(-age))
            age--;

        return age;
    }

    /// <summary>
    /// Valide que l'utilisateur a plus de 14 ans.
    /// </summary>
    /// <param name="birthDate">Date de naissance à valider.</param>
    /// <exception cref="DomainException">Si l'âge est inférieur ou égal à 14 ans.</exception>
    private static void ValidateAge(DateOnly birthDate)
    {
        int age = CalculateAge(birthDate);

        if (age <= MinimumAge)
        {
            throw new DomainException($"Vous devez avoir plus de {MinimumAge} ans pour utiliser cette application.");
        }
    }
}
