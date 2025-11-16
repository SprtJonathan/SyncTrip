using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Core.Entities;

/// <summary>
/// Classe de base pour toutes les entités du domaine
/// Fournit les propriétés communes : Id, CreatedAt, UpdatedAt
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identifiant unique de l'entité
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Date de création de l'entité
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de dernière mise à jour de l'entité
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
