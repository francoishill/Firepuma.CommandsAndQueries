using MediatR;
using Microsoft.Extensions.Logging;

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;

internal class LoggingScopePipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingScopePipelineBehaviour<TRequest, TResponse>> _logger;

    public LoggingScopePipelineBehaviour(
        ILogger<LoggingScopePipelineBehaviour<TRequest, TResponse>> logger)
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