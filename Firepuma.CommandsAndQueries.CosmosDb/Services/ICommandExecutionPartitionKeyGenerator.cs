using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.CosmosDb.Services;

public interface ICommandExecutionPartitionKeyGenerator
{
    string GeneratePartitionKey(CommandExecutionEvent entity);
}