using Firepuma.CommandsAndQueries.MongoDb.Repositories;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Abstractions;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.QuerySpecifications;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Services;

internal class BackgroundEventPublisherService : BackgroundService
{
    private readonly ILogger<BackgroundEventPublisherService> _logger;
    private readonly ICommandExecutionRepository _commandExecutionRepository;
    private readonly ICommandExecutionIntegrationEventPublisher _integrationEventPublisher;

    public BackgroundEventPublisherService(
        ILogger<BackgroundEventPublisherService> logger,
        ICommandExecutionRepository commandExecutionRepository,
        ICommandExecutionIntegrationEventPublisher integrationEventPublisher)
    {
        _logger = logger;
        _commandExecutionRepository = commandExecutionRepository;
        _integrationEventPublisher = integrationEventPublisher;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var querySpecification = new PendingIntegrationEventsQuerySpecification();

                var commandExecutions = await _commandExecutionRepository.GetItemsAsync(querySpecification, stoppingToken);

                foreach (var commandExecution in commandExecutions)
                {
                    try
                    {
                        // important to respect the lock here, since it is background processing
                        const bool ignoreExistingLock = false;

                        await _integrationEventPublisher.PublishEventAsync(commandExecution, ignoreExistingLock, stoppingToken);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(
                            exception,
                            "Unable to publish integration event for command execution document id {DocumentId}, command id {CommandId}",
                            commandExecution.Id, commandExecution.CommandId);
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unable to fetch pending integration events");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}