using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.Abstractions.Commands;

public interface ICommandExecutionDecorator
{
    void Decorate<TResponse>(
        ICommandExecutionEvent executionEvent,
        ICommandRequest command,
        bool successful,
        TResponse? response,
        Exception? error);
}