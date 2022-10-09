using MediatR;

namespace Firepuma.CommandsAndQueries.Abstractions.Authorization;

public interface IAuthorizationRequirement : IRequest<AuthorizationResult>
{
}