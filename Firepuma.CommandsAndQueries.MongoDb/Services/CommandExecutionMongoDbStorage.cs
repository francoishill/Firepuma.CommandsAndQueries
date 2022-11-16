using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using Firepuma.CommandsAndQueries.MongoDb.Entities;
using Firepuma.CommandsAndQueries.MongoDb.Repositories;

// ReSharper disable ArgumentsStyleNamedExpression

// ReSharper disable ClassNeverInstantiated.Global

namespace Firepuma.CommandsAndQueries.MongoDb.Services;

internal class CommandExecutionMongoDbStorage : ICommandExecutionStorage
{
    private readonly ICommandExecutionRepository _commandExecutionRepository;

    public CommandExecutionMongoDbStorage(
        ICommandExecutionRepository commandExecutionRepository)
    {
        _commandExecutionRepository = commandExecutionRepository;
    }

    public ICommandExecutionEvent CreateNewItem(ICommandRequest commandRequest)
    {
        return new CommandExecutionMongoDbEvent();
    }

    public async Task<ICommandExecutionEvent> AddItemAsync(ICommandExecutionEvent executionEvent, CancellationToken cancellationToken)
    {
        if (executionEvent is not CommandExecutionMongoDbEvent executionMongoDbEvent)
        {
            throw new ArgumentException($"Argument {nameof(executionEvent)} is expected to be of type {nameof(CommandExecutionMongoDbEvent)}", nameof(executionEvent));
        }

        return await _commandExecutionRepository.AddItemAsync(
            executionMongoDbEvent,
            cancellationToken);
    }

    public async Task<ICommandExecutionEvent> UpsertItemAsync(ICommandExecutionEvent executionEvent, CancellationToken cancellationToken)
    {
        if (executionEvent is not CommandExecutionMongoDbEvent executionMongoDbEvent)
        {
            throw new ArgumentException($"Argument {nameof(executionEvent)} is expected to be of type {nameof(CommandExecutionMongoDbEvent)}", nameof(executionEvent));
        }

        return await _commandExecutionRepository.UpsertItemAsync(
            executionMongoDbEvent,
            ignoreETag: false,
            cancellationToken: cancellationToken);
    }
}