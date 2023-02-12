using System.Diagnostics;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.DatabaseRepositories.MongoDb.Abstractions.Entities;
using MongoDB.Driver;

#pragma warning disable CS8618
// ReSharper disable EmptyConstructor

namespace Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Entities;

[DebuggerDisplay("{ToString()}")]
public class AuthorizationFailureMongoDbEvent : BaseMongoDbEntity, IAuthorizationFailureEvent
{
    public DateTime CreatedOn { get; set; }

    public string ActionTypeName { get; set; }
    public string ActionTypeNamespace { get; set; }
    public object? ActionPayload { get; set; }
    public FailedAuthorizationRequirement[] FailedRequirements { get; set; }

    public AuthorizationFailureMongoDbEvent()
    {
        // Typically used by Database repository deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public override string ToString()
    {
        return $"{Id}/{ActionTypeName}/{ActionTypeNamespace}";
    }

    public static IEnumerable<CreateIndexModel<AuthorizationFailureMongoDbEvent>> GetSchemaIndexes()
    {
        return Array.Empty<CreateIndexModel<AuthorizationFailureMongoDbEvent>>();
    }
}