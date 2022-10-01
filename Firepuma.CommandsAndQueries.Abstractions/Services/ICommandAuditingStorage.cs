using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.Abstractions.Services;

public interface ICommandAuditingStorage
{
    Task<CommandExecutionEvent> AddItemAsync(CommandExecutionEvent executionEvent, CancellationToken cancellationToken);
    Task<CommandExecutionEvent> UpsertItemAsync(CommandExecutionEvent executionEvent, CancellationToken cancellationToken);
}