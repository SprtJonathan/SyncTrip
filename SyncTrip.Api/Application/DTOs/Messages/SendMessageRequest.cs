using System.ComponentModel.DataAnnotations;

namespace SyncTrip.Api.Application.DTOs.Messages;

/// <summary>
/// Requête d'envoi de message
/// </summary>
public class SendMessageRequest
{
    [Required(ErrorMessage = "Le contenu est obligatoire")]
    [MaxLength(2000, ErrorMessage = "Le message ne peut pas dépasser 2000 caractères")]
    [MinLength(1, ErrorMessage = "Le message ne peut pas être vide")]
    public string Content { get; set; } = string.Empty;
}
