using Firepuma.CommandsAndQueries.CosmosDb.Entities;
using Firepuma.DatabaseRepositories.CosmosDb.Services.Requests;
using Microsoft.Azure.Cosmos;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.CosmosDb.Configuration;

public static class CosmosContainers
{
    public static readonly ContainerSpecification AuthorizationFailures = new()
    {
        ContainerProperties = new ContainerProperties(id: "AuthorizationFailures", partitionKeyPath: $"/{nameof(AuthorizationFailureCosmosDbEvent.PartitionKey)}"),
    };

    public static readonly ContainerSpecification CommandExecutions = new()
    {
        ContainerProperties = new ContainerProperties(id: "CommandExecutions", partitionKeyPath: $"/{nameof(CommandExecutionCosmosDbEvent.PartitionKey)}"),
    };

    public static readonly ContainerSpecification Pets = new()
    {
        ContainerProperties = new ContainerProperties(id: "Pets", partitionKeyPath: $"/{nameof(PetEntity.Type)}"),
    };
}