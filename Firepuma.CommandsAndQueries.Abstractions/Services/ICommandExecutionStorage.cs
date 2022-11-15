using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.Abstractions.Services;

public interface ICommandExecutionStorage
{
    BaseCommandExecutionEvent CreateNewItem(ICommandRequest commandRequest);

    Task<BaseCommandExecutionEvent> AddItemAsync(BaseCommandExecutionEvent executionEvent, CancellationToken cancellationToken);
    Task<BaseCommandExecutionEvent> UpsertItemAsync(BaseCommandExecutionEvent executionEvent, CancellationToken cancellationToken);
}