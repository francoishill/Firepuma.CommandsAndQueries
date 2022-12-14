using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using Firepuma.CommandsAndQueries.CosmosDb.Entities;
using Firepuma.CommandsAndQueries.CosmosDb.Repositories;

// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ClassNeverInstantiated.Global

namespace Firepuma.CommandsAndQueries.CosmosDb.Services;

internal class CommandExecutionCosmosDbStorage : ICommandExecutionStorage
{
    private readonly ICommandExecutionRepository _commandExecutionRepository;

    public CommandExecutionCosmosDbStorage(
        ICommandExecutionRepository commandExecutionRepository)
    {
        _commandExecutionRepository = commandExecutionRepository;
    }

    public ICommandExecutionEvent CreateNewItem(ICommandRequest commandRequest)
    {
        return new CommandExecutionCosmosDbEvent();
    }

    public async Task<ICommandExecutionEvent> AddItemAsync(ICommandExecutionEvent executionEvent, CancellationToken cancellationToken)
    {
        if (executionEvent is not CommandExecutionCosmosDbEvent executionCosmosDbEvent)
        {
            throw new ArgumentException($"Argument {nameof(executionEvent)} is expected to be of type {nameof(CommandExecutionCosmosDbEvent)}", nameof(executionEvent));
        }

        return await _commandExecutionRepository.AddItemAsync(
            executionCosmosDbEvent,
            cancellationToken);
    }

    public async Task<ICommandExecutionEvent> UpsertItemAsync(ICommandExecutionEvent executionEvent, CancellationToken cancellationToken)
    {
        if (executionEvent is not CommandExecutionCosmosDbEvent executionCosmosDbEvent)
        {
            throw new ArgumentException($"Argument {nameof(executionEvent)} is expected to be of type {nameof(CommandExecutionCosmosDbEvent)}", nameof(executionEvent));
        }

        return await _commandExecutionRepository.UpsertItemAsync(
            executionCosmosDbEvent,
            ignoreETag: false,
            cancellationToken: cancellationToken);
    }
}