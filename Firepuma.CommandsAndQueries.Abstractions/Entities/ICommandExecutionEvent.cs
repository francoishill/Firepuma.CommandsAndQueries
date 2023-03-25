#pragma warning disable CS8618

namespace Firepuma.CommandsAndQueries.Abstractions.Entities;

public interface ICommandExecutionEvent
{
    string Id { get; set; }

    DateTime CreatedOn { get; set; }

    string CommandId { get; set; }
    string TypeName { get; set; }
    string TypeNamespace { get; set; }
    string Payload { get; set; }

    bool? Successful { get; set; }
    string? Result { get; set; }
    string? ErrorMessage { get; set; }
    string? ErrorStackTrace { get; set; }
    double? ExecutionTimeInSeconds { get; set; }
    double? TotalTimeInSeconds { get; set; }

    Dictionary<string, object?> ExtraValues { get; set; }
}