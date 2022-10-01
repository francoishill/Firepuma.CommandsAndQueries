using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using Firepuma.CommandsAndQueries.CosmosDb.Repositories;

// ReSharper disable ArgumentsStyleNamedExpression

// ReSharper disable ClassNeverInstantiated.Global

namespace Firepuma.CommandsAndQueries.CosmosDb.Services;

public class CommandAuditingCosmosDbStorage : ICommandAuditingStorage
{
    private readonly ICommandExecutionRepository _commandExecutionRepository;

    public CommandAuditingCosmosDbStorage(
        ICommandExecutionRepository commandExecutionRepository)
    {
        _commandExecutionRepository = commandExecutionRepository;
    }

    public async Task<CommandExecutionEvent> AddItemAsync(CommandExecutionEvent executionEvent, CancellationToken cancellationToken)
    {
        return await _commandExecutionRepository.AddItemAsync(
            executionEvent,
            cancellationToken);
    }

    public async Task<CommandExecutionEvent> UpsertItemAsync(CommandExecutionEvent executionEvent, CancellationToken cancellationToken)
    {
        return await _commandExecutionRepository.UpsertItemAsync(
            executionEvent,
            ignoreETag: false,
            cancellationToken: cancellationToken);
    }
}