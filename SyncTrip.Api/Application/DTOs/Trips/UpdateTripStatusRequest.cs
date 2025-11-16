using System.ComponentModel.DataAnnotations;
using SyncTrip.Api.Core.Enums;

namespace SyncTrip.Api.Application.DTOs.Trips;

/// <summary>
/// Requête de mise à jour du statut d'un trip
/// </summary>
public class UpdateTripStatusRequest
{
    [Required(ErrorMessage = "Le statut est obligatoire")]
    public TripStatus Status { get; set; }
}
