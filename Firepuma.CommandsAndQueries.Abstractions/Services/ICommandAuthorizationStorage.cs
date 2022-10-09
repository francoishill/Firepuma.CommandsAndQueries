using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.Abstractions.Services;

public interface ICommandAuthorizationStorage
{
    Task<AuthorizationFailureEvent> AddItemAsync(AuthorizationFailureEvent executionEvent, CancellationToken cancellationToken);
}