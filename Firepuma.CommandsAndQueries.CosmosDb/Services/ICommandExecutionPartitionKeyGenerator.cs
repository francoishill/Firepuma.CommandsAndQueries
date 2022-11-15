using Firepuma.CommandsAndQueries.CosmosDb.Entities;

namespace Firepuma.CommandsAndQueries.CosmosDb.Services;

public interface ICommandExecutionPartitionKeyGenerator
{
    string GeneratePartitionKey(CommandExecutionCosmosDbEvent entity);
}