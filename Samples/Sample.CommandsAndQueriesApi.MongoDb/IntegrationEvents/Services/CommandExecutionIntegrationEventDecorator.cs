using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Newtonsoft.Json;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Abstractions;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Constants;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Services;

public class CommandExecutionIntegrationEventDecorator : ICommandExecutionDecorator
{
    private readonly ILogger<CommandExecutionIntegrationEventDecorator> _logger;

    public CommandExecutionIntegrationEventDecorator(
        ILogger<CommandExecutionIntegrationEventDecorator> logger)
    {
        _logger = logger;
    }

    public void ExecutionEvent<TResponse>(
        ICommandExecutionEvent executionEvent,
        ICommandRequest command,
        bool successful,
        TResponse? response,
        Exception? error)
    {
        if (!successful)
        {
            _logger.LogInformation(
                "Command execution was not successful for command id {CommandId}, integration event result will be ignored, execution exception message: {Message}",
                command.CommandId, error?.Message);
            return;
        }

        if (response is not BaseIntegrationEventPayload integrationEventPayload)
        {
            _logger.LogDebug(
                "Command execution response is not of type {ExpectedType} for command id {CommandId}, integration event result will be ignored",
                nameof(BaseIntegrationEventPayload), command.CommandId);
            return;
        }

        // it is necessary to store as JSON otherwise will fail to deserialize the objects with mongo type discriminators
        var integrationEventPayloadJson = JsonConvert.SerializeObject(
            integrationEventPayload,
            GetIntegrationEventPayloadSerializerSettings());

        executionEvent.ExtraValues[IntegrationEventExtraValuesKeys.PAYLOAD_JSON] = integrationEventPayloadJson;
    }

    private static JsonSerializerSettings GetIntegrationEventPayloadSerializerSettings()
    {
        var jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return jsonSerializerSettings;
    }
}