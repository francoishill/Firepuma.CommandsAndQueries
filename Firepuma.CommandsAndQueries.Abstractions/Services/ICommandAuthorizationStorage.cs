using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.Abstractions.Services;

public interface ICommandAuthorizationStorage
{
    IAuthorizationFailureEvent CreateNewItem(Type actionType, object actionPayload, FailedAuthorizationRequirement[] failedRequirements);

    Task<IAuthorizationFailureEvent> AddItemAsync(IAuthorizationFailureEvent executionEvent, CancellationToken cancellationToken);
}