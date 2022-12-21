using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.MongoDb.Entities;
using Firepuma.CommandsAndQueries.MongoDb.Repositories;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Abstractions;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Constants;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Services;

internal class CommandExecutionIntegrationEventPublisher : ICommandExecutionIntegrationEventPublisher
{
    private readonly ILogger<CommandExecutionIntegrationEventPublisher> _logger;
    private readonly ICommandExecutionRepository _commandExecutionRepository;

    public CommandExecutionIntegrationEventPublisher(
        ILogger<CommandExecutionIntegrationEventPublisher> logger,
        ICommandExecutionRepository commandExecutionRepository)
    {
        _logger = logger;
        _commandExecutionRepository = commandExecutionRepository;
    }

    public async Task PublishEventAsync(
        ICommandExecutionEvent executionEvent,
        bool ignoreExistingLock,
        CancellationToken cancellationToken)
    {
        if (executionEvent.Successful != true)
        {
            _logger.LogError(
                "{MethodName} should not be called for unsuccessful execution events (command execution document id {DocumentId}, command id {CommandId})",
                nameof(PublishEventAsync), executionEvent.Id, executionEvent.CommandId);
            return;
        }

        if (executionEvent is not CommandExecutionMongoDbEvent commandExecution)
        {
            _logger.LogError(
                "Execution event should be of type {ExpectedType} but is type {ActualType}, command execution document id {DocumentId}, command id {CommandId}",
                nameof(CommandExecutionMongoDbEvent), executionEvent.GetType().FullName, executionEvent.Id, executionEvent.CommandId);
            return;
        }

        if (!commandExecution.ExtraValues.TryGetValue(IntegrationEventExtraValuesKeys.PAYLOAD_JSON, out var eventPayloadJson)
            || eventPayloadJson?.ToString() == null)
        {
            _logger.LogWarning(
                "Unable to extract {PayloadField} from command execution document id {DocumentId}, command id {CommandId}",
                IntegrationEventExtraValuesKeys.PAYLOAD_JSON, commandExecution.Id, commandExecution.CommandId);
            return;
        }

        if (!ignoreExistingLock
            && commandExecution.ExtraValues.TryGetValue(IntegrationEventExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS, out var previousLockUnixSecondsObj))
        {
            DateTimeOffset? lockExpiryDate = null;
            bool? isExpired = null;
            if (previousLockUnixSecondsObj is long previousLockUnixSeconds)
            {
                lockExpiryDate = DateTimeOffset.FromUnixTimeSeconds(previousLockUnixSeconds);
                isExpired = previousLockUnixSeconds < DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            else
            {
                _logger.LogWarning(
                    "Unable to determine whether command execution lock expired, " +
                    "command document id {DocumentId}, command id {CommandId}, previous lock expiry {LockUnixSeconds} unix seconds",
                    commandExecution.Id, commandExecution.CommandId, previousLockUnixSecondsObj);
            }

            if (isExpired == false)
            {
                _logger.LogError(
                    "Command execution lock is not yet expired so aborting this operation, " +
                    "command document id {DocumentId}, command id {CommandId}, previous lock expiry {LockUnixSeconds} unix seconds ({LockExpiryDate})",
                    commandExecution.Id, commandExecution.CommandId, previousLockUnixSecondsObj, lockExpiryDate?.ToString("O"));
                return;
            }

            _logger.LogWarning(
                "Command execution lock expired and not removed, but will published again now, it was probably started by another thread/process that got aborted midway, " +
                "command document id {DocumentId}, command id {CommandId}, previous lock expiry {LockUnixSeconds} unix seconds ({LockExpiryDate})",
                commandExecution.Id, commandExecution.CommandId, previousLockUnixSecondsObj, lockExpiryDate?.ToString("O"));
        }

        try
        {
            // lock for a short while, to ensure we don't have duplicate processing
            var soonUnixSeconds = DateTimeOffset.UtcNow.AddMinutes(2).ToUnixTimeSeconds();
            commandExecution.ExtraValues[IntegrationEventExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS] = soonUnixSeconds;
            await _commandExecutionRepository.UpsertItemAsync(commandExecution, ignoreETag: false, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to obtain lock for publishing integration event for command execution document id {DocumentId}, command id {CommandId}",
                commandExecution.Id, commandExecution.CommandId);

            return;
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

        commandExecution.ExtraValues.Remove(IntegrationEventExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS);
        await _commandExecutionRepository.UpsertItemAsync(commandExecution, ignoreETag: false, cancellationToken);
    }

    private static void SetPublishResult(
        ICommandExecutionEvent commandExecution,
        DateTime dateTime,
        bool isSuccessful,
        PublishError? error)
    {
        commandExecution.ExtraValues[IntegrationEventExtraValuesKeys.PUBLISH_RESULT_TIME] = dateTime;
        commandExecution.ExtraValues[IntegrationEventExtraValuesKeys.PUBLISH_RESULT_SUCCESS] = isSuccessful;

        if (error != null)
        {
            commandExecution.ExtraValues[IntegrationEventExtraValuesKeys.PUBLISH_RESULT_ERROR] = JsonConvert.SerializeObject(error, GetPublishResultSerializerSettings());
        }
    }

    private class PublishError
    {
        public string Message { get; init; } = null!;
        public string? StackTrace { get; init; }
    }

    private static JsonSerializerSettings GetPublishResultSerializerSettings()
    {
        var jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return jsonSerializerSettings;
    }
}