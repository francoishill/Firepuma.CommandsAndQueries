using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors.Helpers;
using MediatR;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Entities;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Repositories;

namespace Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.PipelineBehaviors;

public class CommandExecutionRecordingPipeline<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse?>
    where TRequest : ICommandRequest
{
    private readonly ICommandExecutionRepository _commandExecutionRepository;

    public CommandExecutionRecordingPipeline(
        ICommandExecutionRepository commandExecutionRepository)
    {
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