using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Abstractions;

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

        // we set the lock for ourself as part of the decorator, so ignore it here since another
        // background process should not have started processing it
        const bool ignoreExistingLock = true;

        await _integrationEventPublisher.PublishEventAsync(executionEvent, ignoreExistingLock, cancellationToken);
    }
}