using System.Diagnostics;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.DatabaseRepositories.MongoDb.Entities;

#pragma warning disable CS8618
// ReSharper disable EmptyConstructor

namespace Firepuma.CommandsAndQueries.MongoDb.Entities;

[DebuggerDisplay("{ToString()}")]
public class AuthorizationFailureMongoDbEvent : BaseMongoDbEntity, IAuthorizationFailureEvent
{
    public DateTime CreatedOn { get; set; }

    public string ActionTypeName { get; set; }
    public string ActionTypeNamespace { get; set; }
    public object ActionPayload { get; set; }
    public FailedAuthorizationRequirement[] FailedRequirements { get; set; }

    public AuthorizationFailureMongoDbEvent()
    {
        // used by Azure Cosmos deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public override string ToString()
    {
        return $"{Id}/{ActionTypeName}/{ActionTypeNamespace}";
    }
}