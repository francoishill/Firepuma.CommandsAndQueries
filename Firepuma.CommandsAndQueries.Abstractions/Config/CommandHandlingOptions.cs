namespace Firepuma.CommandsAndQueries.Abstractions.Config;

public class CommandHandlingOptions
{
    /// <summary>
    /// Add logger around MediatR command handlers and add the request type
    /// </summary>
    public bool AddLoggingScopePipelineBehavior { get; set; } = true;

    /// <summary>
    /// Add auditing of each command's execution, this will require ICommandAuditingStorage to have a registered implementation
    /// </summary>
    public bool AddAuditing { get; set; } = true;
}