using Firepuma.CommandsAndQueries.CosmosDb.Entities;
using Firepuma.CommandsAndQueries.CosmosDb.Services;

// ReSharper disable ClassNeverInstantiated.Global

namespace Sample.CommandsAndQueriesApi.CosmosDb.Services;

public class CommandExecutionPartitionKeyGenerator : ICommandExecutionPartitionKeyGenerator
{
    public string GeneratePartitionKey(CommandExecutionCosmosDbEvent entity)
    {
        return entity.CreatedOn.ToString("yyyy-MM");
    }
}