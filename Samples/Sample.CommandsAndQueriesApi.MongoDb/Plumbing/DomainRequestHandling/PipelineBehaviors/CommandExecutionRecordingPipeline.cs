using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors.Helpers;
using MediatR;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Entities;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Repositories;

namespace Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.PipelineBehaviors;

public class CommandExecutionRecordingPipeline<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse?>
    where TRequest : ICommandRequest, IRequest<TResponse?>
{
    private readonly ILogger<CommandExecutionRecordingPipeline<TRequest, TResponse>> _logger;
    private readonly ICommandExecutionRepository _commandExecutionRepository;

    public CommandExecutionRecordingPipeline(
        ILogger<CommandExecutionRecordingPipeline<TRequest, TResponse>> logger,
        ICommandExecutionRepository commandExecutionRepository)
    {
        _logger = logger;
        _commandExecutionRepository = commandExecutionRepository;
    }

    public async Task<TResponse?> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse?> next,
        CancellationToken cancellationToken)
    {
        var executionEvent = new CommandExecutionMongoDbEvent
        {
            Id = CommandExecutionMongoDbEvent.GenerateId(),
        };

        CommandExecutionHelpers.PopulateExecutionEventBeforeStart(request, executionEvent);

        executionEvent = await _commandExecutionRepository.AddItemAsync(
            executionEvent,
            cancellationToken);

        try
        {
            return await CommandExecutionHelpers.ExecuteCommandAsync(
                next,
                request,
                executionEvent);
        }
        finally
        {
            // ReSharper disable once RedundantAssignment
            executionEvent = await _commandExecutionRepository.UpsertItemAsync(
                executionEvent,
                ignoreETag: false,
                cancellationToken);
        }
    }
}