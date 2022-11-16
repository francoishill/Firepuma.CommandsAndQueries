using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ReSharper disable RedundantAssignment

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;

internal class CommandExecutionRecordingPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse?> where TRequest : IRequest<TResponse?>
{
    private readonly ILogger<CommandExecutionRecordingPipeline<TRequest, TResponse>> _logger;
    private readonly ICommandExecutionStorage _commandExecutionStorage;

    public CommandExecutionRecordingPipeline(
        ILogger<CommandExecutionRecordingPipeline<TRequest, TResponse>> logger,
        ICommandExecutionStorage commandExecutionStorage)
    {
        _logger = logger;
        _commandExecutionStorage = commandExecutionStorage;
    }

    public async Task<TResponse?> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse?> next,
        CancellationToken cancellationToken)
    {
        if (request is ICommandRequest commandRequest)
        {
            var response = await ExecuteAndRecord(next, commandRequest, cancellationToken);
            return response;
        }

        return await next();
    }

    private async Task<TResponse?> ExecuteAndRecord(
        RequestHandlerDelegate<TResponse?> next,
        ICommandRequest commandRequest,
        CancellationToken cancellationToken)
    {
        var executionEvent = _commandExecutionStorage.CreateNewItem(commandRequest);

        executionEvent.CreatedOn = commandRequest.CreatedOn;

        executionEvent.CommandId = commandRequest.CommandId;
        executionEvent.TypeName = CommandExecutionEventHelpers.GetTypeName(commandRequest);
        executionEvent.TypeNamespace = CommandExecutionEventHelpers.GetTypeNamespace(commandRequest);
        executionEvent.Payload = CommandExecutionEventHelpers.GetSerializedPayload(commandRequest);

        executionEvent = await _commandExecutionStorage.AddItemAsync(executionEvent, cancellationToken);

        var startTime = DateTime.UtcNow;

        TResponse? response = default;

        string? result = null;
        Exception? error = null;
        bool successful;
        try
        {
            response = await next();
            result = response != null ? JsonConvert.SerializeObject(response, GetRecordResponseSerializerSettings()) : null;
            successful = true;
        }
        catch (Exception exception)
        {
            error = exception;
            successful = false;
            _logger.LogError(exception, "Failed to execute command type {Type}", commandRequest.GetType().FullName);
        }

        var finishedTime = DateTime.UtcNow;

        executionEvent.Successful = successful;
        executionEvent.Result = result;
        executionEvent.ErrorMessage = error?.Message;
        executionEvent.ErrorStackTrack = error?.StackTrace;
        executionEvent.ExecutionTimeInSeconds = (finishedTime - startTime).TotalSeconds;
        executionEvent.TotalTimeInSeconds = (finishedTime - commandRequest.CreatedOn).TotalSeconds;

        executionEvent = await _commandExecutionStorage.UpsertItemAsync(executionEvent, cancellationToken);

        if (error != null)
        {
            throw error;
        }

        return response;
    }

    private static JsonSerializerSettings GetRecordResponseSerializerSettings()
    {
        var jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return jsonSerializerSettings;
    }
}