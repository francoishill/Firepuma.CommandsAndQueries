using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.MongoDb.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Abstractions;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Constants;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.QuerySpecifications;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Services;

public class BackgroundEventPublisherService : BackgroundService
{
    private readonly ILogger<BackgroundEventPublisherService> _logger;
    private readonly ICommandExecutionRepository _commandExecutionRepository;

    public BackgroundEventPublisherService(
        ILogger<BackgroundEventPublisherService> logger,
        ICommandExecutionRepository commandExecutionRepository)
    {
        _logger = logger;
        _commandExecutionRepository = commandExecutionRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var querySpecification = new PendingIntegrationEventsQuerySpecification();

                var items = await _commandExecutionRepository.GetItemsAsync(querySpecification, stoppingToken);

                foreach (var commandExecution in items)
                {
                    if (!commandExecution.ExtraValues.TryGetValue(ExtraValuesKeys.PAYLOAD_JSON, out var eventPayloadJson)
                        || eventPayloadJson?.ToString() == null)
                    {
                        _logger.LogWarning(
                            "Unable to extract {PayloadField} from command execution document id {DocumentId}, command id {CommandId}",
                            ExtraValuesKeys.PAYLOAD_JSON, commandExecution.Id, commandExecution.CommandId);
                        continue;
                    }

                    if (commandExecution.ExtraValues.TryGetValue(ExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS, out var previousLockUnixSecondsObj))
                    {
                        DateTimeOffset? lockExpiryDate = null;
                        if (previousLockUnixSecondsObj is long previousLockUnixSeconds)
                        {
                            lockExpiryDate = DateTimeOffset.FromUnixTimeSeconds(previousLockUnixSeconds);
                        }

                        _logger.LogWarning(
                            "Command execution lock expired but will get published again now (it was probably started by another thread/process), " +
                            "command document id {DocumentId}, command id {CommandId}, previous lock expiry {LockUnixSeconds} unix seconds ({LockExpiryDate})",
                            commandExecution.Id, commandExecution.CommandId, previousLockUnixSecondsObj, lockExpiryDate?.ToString("O"));
                    }

                    try
                    {
                        // lock for a short while, to ensure we don't have duplicate processing
                        var soonUnixSeconds = DateTimeOffset.UtcNow.AddMinutes(2).ToUnixTimeSeconds();
                        commandExecution.ExtraValues[ExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS] = soonUnixSeconds;
                        await _commandExecutionRepository.UpsertItemAsync(commandExecution, ignoreETag: false, stoppingToken);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogWarning(
                            exception,
                            "Failed to obtain lock for publishing integration event for command execution document id {DocumentId}, command id {CommandId}",
                            commandExecution.Id, commandExecution.CommandId);

                        continue;
                    }

                    try
                    {
                        var eventPayload = JsonConvert.DeserializeObject<JObject>(eventPayloadJson.ToString()!);
                        if (eventPayload == null)
                        {
                            SetPublishResult(
                                commandExecution,
                                DateTime.UtcNow,
                                false,
                                new PublishError
                                {
                                    Message = "Unable to deserialize event payload as JObject, its result was null",
                                });
                        }
                        else
                        {
                            var integrationEventId = eventPayload[nameof(BaseIntegrationEventPayload.IntegrationEventId)]?.Value<string>();
                            if (!string.IsNullOrWhiteSpace(integrationEventId))
                            {
                                _logger.LogWarning("TODO: publish integration event here for event id {Id}, will now mark it published", integrationEventId);

                                SetPublishResult(commandExecution, DateTime.UtcNow, true, null);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        SetPublishResult(
                            commandExecution,
                            DateTime.UtcNow,
                            false,
                            new PublishError
                            {
                                Message = exception.Message,
                                StackTrace = exception.StackTrace,
                            });
                    }

                    commandExecution.ExtraValues.Remove(ExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS);
                    await _commandExecutionRepository.UpsertItemAsync(commandExecution, ignoreETag: false, stoppingToken);
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

    private static void SetPublishResult(
        ICommandExecutionEvent commandExecution,
        DateTime dateTime,
        bool isSuccessful,
        PublishError? error)
    {
        commandExecution.ExtraValues[ExtraValuesKeys.PUBLISH_RESULT_TIME] = dateTime;
        commandExecution.ExtraValues[ExtraValuesKeys.PUBLISH_RESULT_SUCCESS] = isSuccessful;

        if (error != null)
        {
            commandExecution.ExtraValues[ExtraValuesKeys.PUBLISH_RESULT_ERROR] = JsonConvert.SerializeObject(error, GetPublishResultSerializerSettings());
        }
    }

    private class PublishError
    {
        public string Message { get; set; } = null!;
        public string? StackTrace { get; set; }
    }

    private static JsonSerializerSettings GetPublishResultSerializerSettings()
    {
        var jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return jsonSerializerSettings;
    }
}