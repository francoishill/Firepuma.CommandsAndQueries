using Firepuma.CommandsAndQueries.Abstractions.Entities;

// ReSharper disable EmptyConstructor

namespace Firepuma.CommandsAndQueries.CosmosDb.Entities;

public class AuthorizationFailureCosmosDbEvent : BaseAuthorizationFailureEvent
{
    public string PartitionKey { get; set; } = null!;

    public AuthorizationFailureCosmosDbEvent()
    {
        // used by Azure Cosmos deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public AuthorizationFailureCosmosDbEvent(
        Type actionType,
        object actionPayload,
        FailedRequirement[] failedRequirements)
        : base(actionType, actionPayload, failedRequirements)
    {
    }
}