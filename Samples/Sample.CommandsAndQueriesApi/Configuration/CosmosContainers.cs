using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Microsoft.Azure.Cosmos;
using Sample.CommandsAndQueriesApi.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.Configuration;

public static class CosmosContainers
{
    public static readonly ContainerProperties CommandExecutions = new(id: "CommandExecutions", partitionKeyPath: $"/{nameof(CommandExecutionEvent.PartitionKey)}");
    public static readonly ContainerProperties Pets = new(id: "Pets", partitionKeyPath: $"/{nameof(PetEntity.Type)}");
}