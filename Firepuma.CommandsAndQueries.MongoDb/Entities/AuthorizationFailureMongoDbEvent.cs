using Firepuma.CommandsAndQueries.Abstractions.Entities;

// ReSharper disable EmptyConstructor

namespace Firepuma.CommandsAndQueries.MongoDb.Entities;

public class AuthorizationFailureMongoDbEvent : BaseAuthorizationFailureEvent
{
    public AuthorizationFailureMongoDbEvent()
    {
        // used by Azure Cosmos deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public AuthorizationFailureMongoDbEvent(
        Type actionType,
        object actionPayload,
        FailedRequirement[] failedRequirements)
        : base(actionType, actionPayload, failedRequirements)
    {
    }
}