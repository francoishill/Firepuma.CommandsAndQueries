using Firepuma.CommandsAndQueries.Abstractions.Config;

namespace Firepuma.CommandsAndQueries.CosmosDb.Config;

public class CosmosDbCommandHandlingOptions : CommandHandlingOptions
{
    #region Authorization

    /// <summary>
    /// The CosmosDb container name for command authorization failure events
    /// </summary>
    public string AuthorizationFailureEventContainerName { get; set; }

    /// <summary>
    /// The service implementation to use as partition key generator for storing authorization failure events
    /// </summary>
    public Type AuthorizationFailurePartitionKeyGenerator { get; set; }

    #endregion

    #region Auditing

    /// <summary>
    /// The CosmosDb container name for command execution events
    /// </summary>
    public string CommandExecutionEventContainerName { get; set; }

    /// <summary>
    /// The service implementation to use as partition key generator for storing command auditing
    /// </summary>
    public Type CommandAuditPartitionKeyGenerator { get; set; }

    #endregion
}