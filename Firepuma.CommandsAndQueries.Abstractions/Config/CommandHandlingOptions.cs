using System.Reflection;

namespace Firepuma.CommandsAndQueries.Abstractions.Config;

public class CommandHandlingOptions
{
    #region Decorative options

    /// <summary>
    /// Catch, log and wrap command exceptions into a CommandException type, then the single exception type can be caught by the caller of the command
    /// </summary>
    public bool AddWrapCommandExceptionsPipelineBehavior { get; set; } = true;

    /// <summary>
    /// Add logger around MediatR command handlers and add the request type
    /// </summary>
    public bool AddLoggingScopePipelineBehavior { get; set; } = true;

    /// <summary>
    /// Add performance logging
    /// </summary>
    public bool AddPerformanceLoggingPipelineBehavior { get; set; } = true;

    #endregion

    #region Validation options

    /// <summary>
    /// Add validation of commands, validation handlers must extend AbstractValidator or IValidator
    /// </summary>
    public bool AddValidationBehaviorPipeline { get; set; } = true;

    /// <summary>
    /// The assemblies to scan for command validation handlers
    /// </summary>
    public Assembly[] ValidationHandlerMarkerAssemblies { get; set; }

    #endregion

    #region Authorization

    /// <summary>
    /// Add authorization of commands, authorization handlers must extend AbstractRequestAuthorizer or IAuthorizer
    /// </summary>
    public bool AddAuthorizationBehaviorPipeline { get; set; } = true;

    /// <summary>
    /// The assemblies to scan for command authorization handlers
    /// </summary>
    public Assembly[] AuthorizationHandlerMarkerAssemblies { get; set; }

    #endregion

    #region Execution recording

    /// <summary>
    /// Add recording of each command's execution, this will require ICommandExecutionStorage to have a registered implementation
    /// </summary>
    public bool AddRecordingOfExecution { get; set; } = true;

    #endregion
}