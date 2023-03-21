using System.Diagnostics;
using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;

public class PerformanceLogPipeline<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceLogPipeline<TRequest, TResponse>> _logger;

    public PerformanceLogPipeline(ILogger<PerformanceLogPipeline<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestTypeName = BehaviorPipelineHelpers.GetShortTypeName(typeof(TRequest));

        var stopwatch = Stopwatch.StartNew();
        _logger.LogDebug(
            "Starting request {Name}",
            requestTypeName);

        var response = await next();

        stopwatch.Stop();
        var durationInSeconds = stopwatch.Elapsed.TotalSeconds.ToString("F");

        var responseTypeName = BehaviorPipelineHelpers.GetShortTypeName(typeof(TResponse));
        _logger.LogDebug(
            "Finished request {Name} (with response type {ResponseType}) in {Duration}s",
            requestTypeName, responseTypeName, durationInSeconds);

        return response;
    }
}