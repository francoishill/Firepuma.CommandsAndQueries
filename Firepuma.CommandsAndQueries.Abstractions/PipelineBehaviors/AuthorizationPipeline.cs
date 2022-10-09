using Firepuma.CommandsAndQueries.Abstractions.Authorization;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Exceptions;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;

internal class AuthorizationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<AuthorizationPipeline<TRequest, TResponse>> _logger;
    private readonly List<IAuthorizer<TRequest>> _authorizers;
    private readonly IMediator _mediator;
    private readonly ICommandAuthorizationStorage _commandAuthorizationStorage;

    public AuthorizationPipeline(
        ILogger<AuthorizationPipeline<TRequest, TResponse>> logger,
        IEnumerable<IAuthorizer<TRequest>> authorizers,
        IMediator mediator,
        ICommandAuthorizationStorage commandAuthorizationStorage)
    {
        _logger = logger;
        _authorizers = authorizers.ToList();
        _mediator = mediator;
        _commandAuthorizationStorage = commandAuthorizationStorage;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_authorizers.Any())
        {
            return await next();
        }

        _logger.LogDebug("Authorizing request '{Type}' with {AuthorizerCount} authorizers", request.GetType().FullName, _authorizers.Count);

        var requirements = new List<IAuthorizationRequirement>();

        foreach (var authorizer in _authorizers)
        {
            await authorizer.BuildPolicy(request, cancellationToken);
            requirements.AddRange(authorizer.Requirements);
        }

        _logger.LogDebug("Request '{Type}' has {Count} authorization requirements", request.GetType().FullName, requirements.Count);

        var failedRequirements = new List<AuthorizationFailureEvent.FailedRequirement>();

        foreach (var requirement in requirements.Distinct())
        {
            var result = await _mediator.Send(requirement, cancellationToken);

            if (!result.IsAuthorized)
            {
                _logger.LogDebug("Requirement '{Requirement}' is not met for request type '{Type}', failure message: '{Message}'",
                    requirement.GetType().FullName, request.GetType().FullName, result.FailureMessage);

                failedRequirements.Add(new AuthorizationFailureEvent.FailedRequirement(
                    requirement,
                    result.FailureMessage));
            }
            else
            {
                _logger.LogDebug("Requirement '{Requirement}' successfully passed for request type '{Type}'",
                    requirement.GetType().FullName, request.GetType().FullName);
            }
        }

        if (failedRequirements.Count > 0)
        {
            await StoreAuthorizationFailedEvent(request, failedRequirements, cancellationToken);

            var combinedMessage = string.Join(". ", failedRequirements.Select(f => f.Message));

            throw new AuthorizationException(combinedMessage);
        }

        return await next();
    }

    private async Task StoreAuthorizationFailedEvent(
        TRequest request,
        List<AuthorizationFailureEvent.FailedRequirement> failedRequirements,
        CancellationToken cancellationToken)
    {
        try
        {
            var authorizationFailureEvent = new AuthorizationFailureEvent(
                request.GetType(),
                request,
                failedRequirements.ToArray());

            await _commandAuthorizationStorage.AddItemAsync(authorizationFailureEvent, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unable to store authorization failed event, error: {Error}", exception.Message);
        }
    }
}