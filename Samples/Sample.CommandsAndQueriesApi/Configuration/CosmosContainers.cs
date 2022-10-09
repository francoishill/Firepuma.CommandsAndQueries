using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.DatabaseRepositories.CosmosDb.Services.Requests;
using Microsoft.Azure.Cosmos;
using Sample.CommandsAndQueriesApi.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.Configuration;

public static class CosmosContainers
{
    public static readonly ContainerSpecification AuthorizationFailures = new()
    {
        ContainerProperties = new ContainerProperties(id: "AuthorizationFailures", partitionKeyPath: $"/{nameof(AuthorizationFailureEvent.PartitionKey)}"),
    };

    public static readonly ContainerSpecification CommandExecutions = new()
    {
        ContainerProperties = new ContainerProperties(id: "CommandExecutions", partitionKeyPath: $"/{nameof(CommandExecutionEvent.PartitionKey)}"),
    };

    public static readonly ContainerSpecification Pets = new()
    {
        ContainerProperties = new ContainerProperties(id: "Pets", partitionKeyPath: $"/{nameof(PetEntity.Type)}"),
    };
}