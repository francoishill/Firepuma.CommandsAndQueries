using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.Abstractions.Services;

public interface ICommandAuthorizationStorage
{
    BaseAuthorizationFailureEvent CreateNewItem(Type actionType, object actionPayload, BaseAuthorizationFailureEvent.FailedRequirement[] failedRequirements);

    Task<BaseAuthorizationFailureEvent> AddItemAsync(BaseAuthorizationFailureEvent executionEvent, CancellationToken cancellationToken);
}