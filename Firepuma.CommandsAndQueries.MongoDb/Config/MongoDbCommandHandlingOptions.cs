using Firepuma.CommandsAndQueries.Abstractions.Config;

namespace Firepuma.CommandsAndQueries.MongoDb.Config;

public class MongoDbCommandHandlingOptions : CommandHandlingOptions
{
    #region Authorization

    /// <summary>
    /// The MongoDb collection name for command authorization failure events
    /// </summary>
    public string AuthorizationFailureEventCollectionName { get; set; } = null!;

    #endregion

    #region Execution recording

    /// <summary>
    /// The MongoDb collection name for command execution events
    /// </summary>
    public string CommandExecutionEventCollectionName { get; set; } = null!;

    #endregion
}