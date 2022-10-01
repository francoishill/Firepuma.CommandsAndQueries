using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ReSharper disable RedundantAssignment

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;

internal class AuditCommandsBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<AuditCommandsBehaviour<TRequest, TResponse>> _logger;
    private readonly ICommandAuditingStorage _commandAuditingStorage;

    public AuditCommandsBehaviour(
        ILogger<AuditCommandsBehaviour<TRequest, TResponse>> logger,
        ICommandAuditingStorage commandAuditingStorage)
    {
        _logger = logger;
        _commandAuditingStorage = commandAuditingStorage;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is ICommandRequest commandRequest)
        {
            return await ExecuteAndAudit(next, commandRequest, cancellationToken);
        }

        return await next();
    }

    private async Task<TResponse> ExecuteAndAudit(
        RequestHandlerDelegate<TResponse> next,
        ICommandRequest commandRequest,
        CancellationToken cancellationToken)
    {
        var executionEvent = new CommandExecutionEvent(commandRequest);
        executionEvent = await _commandAuditingStorage.AddItemAsync(executionEvent, cancellationToken);

        var startTime = DateTime.UtcNow;

        TResponse response = default;

        string result = null;
        Exception error = null;
        bool successful;
        try
        {
            response = await next();
            result = response != null ? JsonConvert.SerializeObject(response, GetAuditResponseSerializerSettings()) : null;
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

        executionEvent = await _commandAuditingStorage.UpsertItemAsync(executionEvent, cancellationToken);

        if (error != null)
        {
            throw error;
        }

        return response;
    }

    private static JsonSerializerSettings GetAuditResponseSerializerSettings()
    {
        var jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        return jsonSerializerSettings;
    }
}