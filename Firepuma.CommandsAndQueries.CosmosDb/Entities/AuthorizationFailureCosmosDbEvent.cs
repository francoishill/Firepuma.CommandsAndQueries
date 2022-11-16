using System.Diagnostics;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.DatabaseRepositories.CosmosDb.Abstractions.Entities;

#pragma warning disable CS8618
// ReSharper disable EmptyConstructor

namespace Firepuma.CommandsAndQueries.CosmosDb.Entities;

[DebuggerDisplay("{ToString()}")]
public class AuthorizationFailureCosmosDbEvent : BaseCosmosDbEntity, IAuthorizationFailureEvent
{
    public string PartitionKey { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string ActionTypeName { get; set; }
    public string ActionTypeNamespace { get; set; }
    public object ActionPayload { get; set; }
    public FailedAuthorizationRequirement[] FailedRequirements { get; set; }

    public AuthorizationFailureCosmosDbEvent()
    {
        // used by Azure Cosmos deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public override string ToString()
    {
        return $"{Id}/{ActionTypeName}/{ActionTypeNamespace}";
    }
}