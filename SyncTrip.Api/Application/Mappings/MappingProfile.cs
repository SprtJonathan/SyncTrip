using AutoMapper;
using SyncTrip.Api.Application.DTOs.Auth;
using SyncTrip.Api.Application.DTOs.Convoys;
using SyncTrip.Api.Application.DTOs.Locations;
using SyncTrip.Api.Application.DTOs.Messages;
using SyncTrip.Api.Application.DTOs.Trips;
using SyncTrip.Api.Application.DTOs.Waypoints;
using SyncTrip.Api.Core.Entities;

namespace SyncTrip.Api.Application.Mappings;

/// <summary>
/// Profil AutoMapper pour tous les mappings Entity ↔ DTO
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ===== USER MAPPINGS =====
        CreateMap<User, UserDto>();
        CreateMap<RegisterRequest, User>();

        // ===== CONVOY MAPPINGS =====
        CreateMap<Convoy, ConvoyDto>()
            .ForMember(dest => dest.ParticipantsCount,
                opt => opt.MapFrom(src => src.Participants.Count(p => p.IsActive)));

        CreateMap<CreateConvoyRequest, Convoy>();
        CreateMap<UpdateConvoyRequest, Convoy>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // ===== CONVOY PARTICIPANT MAPPINGS =====
        CreateMap<ConvoyParticipant, ConvoyParticipantDto>()
            .ForMember(dest => dest.UserDisplayName,
                opt => opt.MapFrom(src => src.User.DisplayName));

        // ===== TRIP MAPPINGS =====
        CreateMap<Trip, TripDto>();
        CreateMap<CreateTripRequest, Trip>();
        CreateMap<UpdateTripStatusRequest, Trip>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        // ===== WAYPOINT MAPPINGS =====
        CreateMap<Waypoint, WaypointDto>();
        CreateMap<CreateWaypointRequest, Waypoint>();

        // ===== MESSAGE MAPPINGS =====
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.UserDisplayName,
                opt => opt.MapFrom(src => src.User != null ? src.User.DisplayName : "Système"));

        CreateMap<SendMessageRequest, Message>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));

        // ===== LOCATION MAPPINGS =====
        CreateMap<LocationHistory, LocationDto>()
            .ForMember(dest => dest.UserDisplayName,
                opt => opt.MapFrom(src => src.User.DisplayName));

        CreateMap<UpdateLocationRequest, LocationHistory>()
            .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}
