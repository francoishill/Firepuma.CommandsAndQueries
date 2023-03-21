using MediatR;
using Microsoft.Extensions.Logging;

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;

public class LoggingScopePipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<LoggingScopePipeline<TRequest, TResponse>> _logger;

    public LoggingScopePipeline(
        ILogger<LoggingScopePipeline<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using (_logger.BeginScope("CommandRequestType:{Type}", request.GetType().FullName))
        {
            return await next();
        }
    }
}