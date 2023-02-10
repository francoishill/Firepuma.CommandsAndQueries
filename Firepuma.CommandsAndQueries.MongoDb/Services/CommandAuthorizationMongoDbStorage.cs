using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using Firepuma.CommandsAndQueries.MongoDb.Entities;
using Firepuma.CommandsAndQueries.MongoDb.Repositories;

namespace Firepuma.CommandsAndQueries.MongoDb.Services;

internal class CommandAuthorizationMongoDbStorage : ICommandAuthorizationStorage
{
    private readonly IAuthorizationFailureEventRepository _authorizationFailureEventRepository;

    public CommandAuthorizationMongoDbStorage(
        IAuthorizationFailureEventRepository authorizationFailureEventRepository)
    {
        _authorizationFailureEventRepository = authorizationFailureEventRepository;
    }

    public IAuthorizationFailureEvent CreateNewItem(Type actionType, object? actionPayload, FailedAuthorizationRequirement[] failedRequirements)
    {
        return new AuthorizationFailureMongoDbEvent();
    }

    public async Task<IAuthorizationFailureEvent> AddItemAsync(IAuthorizationFailureEvent executionEvent, CancellationToken cancellationToken)
    {
        if (executionEvent is not AuthorizationFailureMongoDbEvent executionMongoDbEvent)
        {
            throw new ArgumentException($"Argument {nameof(executionEvent)} is expected to be of type {nameof(AuthorizationFailureMongoDbEvent)}", nameof(executionEvent));
        }

        return await _authorizationFailureEventRepository.AddItemAsync(
            executionMongoDbEvent,
            cancellationToken);
    }
}