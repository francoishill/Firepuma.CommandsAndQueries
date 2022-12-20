using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.IntegrationEvents;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Services;

internal class CommandExecutionPostProcessor : ICommandExecutionPostProcessor
{
    private readonly ICommandExecutionIntegrationEventPublisher _integrationEventPublisher;

    public CommandExecutionPostProcessor(
        ICommandExecutionIntegrationEventPublisher integrationEventPublisher)
    {
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task ProcessAsync<TResponse>(
        ICommandExecutionEvent executionEvent,
        ICommandRequest command,
        bool successful,
        TResponse? response,
        Exception? error,
        CancellationToken cancellationToken)
    {
        if (!successful)
        {
            return;
        }

        await _integrationEventPublisher.PublishEventAsync(executionEvent, cancellationToken);
    }
}