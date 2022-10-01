using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.CosmosDb.Services;

public interface ICommandAuditPartitionKeyGenerator
{
    string GeneratePartitionKey(CommandExecutionEvent entity);
}