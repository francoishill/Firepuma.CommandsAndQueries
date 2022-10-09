using Firepuma.CommandsAndQueries.Abstractions.Config;

namespace Firepuma.CommandsAndQueries.CosmosDb.Config;

public class CosmosDbCommandHandlingOptions : CommandHandlingOptions
{
    /// <summary>
    /// The CosmosDb container name for command execution events
    /// </summary>
    public string CommandExecutionEventContainerName { get; set; }

    /// <summary>
    /// The service implementation to use as partition key generator
    /// </summary>
    public Type CommandAuditPartitionKeyGenerator { get; set; }
}