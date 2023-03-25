using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using MediatR;
using Newtonsoft.Json;

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors.Helpers;

public static class CommandExecutionHelpers
{
    public static void PopulateExecutionEventBeforeStart(
        ICommandRequest commandRequest,
        ICommandExecutionEvent executionEvent)
    {
        executionEvent.CreatedOn = commandRequest.CreatedOn;

        executionEvent.CommandId = commandRequest.CommandId;
        executionEvent.TypeName = CommandExecutionEventHelpers.GetTypeName(commandRequest);
        executionEvent.TypeNamespace = CommandExecutionEventHelpers.GetTypeNamespace(commandRequest);
        executionEvent.Payload = CommandExecutionEventHelpers.GetSerializedPayload(commandRequest);
    }

    public static async Task<TResponse?> ExecuteCommandAsync<TResponse>(
        RequestHandlerDelegate<TResponse?> next,
        ICommandRequest commandRequest,
        ICommandExecutionEvent executionEvent)
    {
        var startTime = DateTime.UtcNow;

        string? result = null;
        Exception? error = null;
        var successful = false;
        try
        {
            var response = await next();
            result = response != null ? JsonConvert.SerializeObject(response, GetRecordResponseSerializerSettings()) : null;
            successful = true;

            return response;
        }
        catch (Exception exception)
        {
            error = exception;
            successful = false;

            throw;
        }
        finally
        {
            var finishedTime = DateTime.UtcNow;

            executionEvent.Successful = successful;
            executionEvent.Result = result;
            executionEvent.ErrorMessage = error?.Message;
            executionEvent.ErrorStackTrace = error?.StackTrace;
            executionEvent.ExecutionTimeInSeconds = (finishedTime - startTime).TotalSeconds;
            executionEvent.TotalTimeInSeconds = (finishedTime - commandRequest.CreatedOn).TotalSeconds;
        }
    }

    private static JsonSerializerSettings GetRecordResponseSerializerSettings()
    {
        var jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return jsonSerializerSettings;
    }
}