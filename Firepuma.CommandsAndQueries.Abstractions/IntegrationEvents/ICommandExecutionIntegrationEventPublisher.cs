using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.Abstractions.IntegrationEvents;

public interface ICommandExecutionIntegrationEventPublisher
{
    Task PublishEventAsync(
        ICommandExecutionEvent executionEvent,
        CancellationToken cancellationToken);
}