using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using Firepuma.CommandsAndQueries.CosmosDb.Repositories;

namespace Firepuma.CommandsAndQueries.CosmosDb.Services;

public class CommandAuthorizationCosmosDbStorage : ICommandAuthorizationStorage
{
    private readonly IAuthorizationFailureEventRepository _authorizationFailureEventRepository;

    public CommandAuthorizationCosmosDbStorage(
        IAuthorizationFailureEventRepository authorizationFailureEventRepository)
    {
        _authorizationFailureEventRepository = authorizationFailureEventRepository;
    }

    public async Task<AuthorizationFailureEvent> AddItemAsync(AuthorizationFailureEvent executionEvent, CancellationToken cancellationToken)
    {
        return await _authorizationFailureEventRepository.AddItemAsync(
            executionEvent,
            cancellationToken);
    }
}