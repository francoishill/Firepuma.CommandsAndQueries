using MediatR;
using Microsoft.Extensions.Logging;

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;

internal class LoggingScopeBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingScopeBehaviour<TRequest, TResponse>> _logger;

    public LoggingScopeBehaviour(
        ILogger<LoggingScopeBehaviour<TRequest, TResponse>> logger)
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