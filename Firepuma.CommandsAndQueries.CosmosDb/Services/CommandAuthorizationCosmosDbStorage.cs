using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using Firepuma.CommandsAndQueries.CosmosDb.Entities;
using Firepuma.CommandsAndQueries.CosmosDb.Repositories;

namespace Firepuma.CommandsAndQueries.CosmosDb.Services;

internal class CommandAuthorizationCosmosDbStorage : ICommandAuthorizationStorage
{
    private readonly IAuthorizationFailureEventRepository _authorizationFailureEventRepository;

    public CommandAuthorizationCosmosDbStorage(
        IAuthorizationFailureEventRepository authorizationFailureEventRepository)
    {
        _authorizationFailureEventRepository = authorizationFailureEventRepository;
    }

    public BaseAuthorizationFailureEvent CreateNewItem(Type actionType, object actionPayload, BaseAuthorizationFailureEvent.FailedRequirement[] failedRequirements)
    {
        return new AuthorizationFailureCosmosDbEvent(actionType, actionPayload, failedRequirements.ToArray());
    }

    public async Task<BaseAuthorizationFailureEvent> AddItemAsync(BaseAuthorizationFailureEvent executionEvent, CancellationToken cancellationToken)
    {
        if (executionEvent is not AuthorizationFailureCosmosDbEvent executionCosmosDbEvent)
        {
            throw new ArgumentException($"Argument {nameof(executionEvent)} is expected to be of type {nameof(AuthorizationFailureCosmosDbEvent)}", nameof(executionEvent));
        }

        return await _authorizationFailureEventRepository.AddItemAsync(
            executionCosmosDbEvent,
            cancellationToken);
    }
}