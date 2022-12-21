using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Abstractions;

internal interface ICommandExecutionIntegrationEventPublisher
{
    Task PublishEventAsync(
        ICommandExecutionEvent executionEvent,
        bool ignoreExistingLock,
        CancellationToken cancellationToken);
}