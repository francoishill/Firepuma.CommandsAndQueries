using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.Abstractions.Services;

public interface ICommandExecutionStorage
{
    ICommandExecutionEvent CreateNewItem(ICommandRequest commandRequest);

    Task<ICommandExecutionEvent> AddItemAsync(ICommandExecutionEvent executionEvent, CancellationToken cancellationToken);
    Task<ICommandExecutionEvent> UpsertItemAsync(ICommandExecutionEvent executionEvent, CancellationToken cancellationToken);
}