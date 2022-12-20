using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.Abstractions.Commands;

public interface ICommandExecutionPostProcessor
{
    Task ProcessAsync<TResponse>(
        ICommandExecutionEvent executionEvent,
        ICommandRequest command,
        bool successful,
        TResponse? response,
        Exception? error,
        CancellationToken cancellationToken);
}