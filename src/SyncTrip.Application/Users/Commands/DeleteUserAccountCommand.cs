using MediatR;

namespace SyncTrip.Application.Users.Commands;

public record DeleteUserAccountCommand(Guid UserId) : IRequest;
