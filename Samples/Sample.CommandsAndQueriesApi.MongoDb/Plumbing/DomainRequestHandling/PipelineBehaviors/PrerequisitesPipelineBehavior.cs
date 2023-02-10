using Firepuma.CommandsAndQueries.Abstractions.Authorization;
using Firepuma.CommandsAndQueries.Abstractions.DomainRequests;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Exceptions;
using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors.Helpers;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using FluentValidation;
using MediatR;

// ReSharper disable InvertIf

namespace Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.PipelineBehaviors;

public class PrerequisitesPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IDomainRequest, IRequest<TResponse>
{
    private readonly ILogger<PrerequisitesPipelineBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;
    private readonly IMediator _mediator;
    private readonly ICommandAuthorizationStorage _commandAuthorizationStorage;

    public PrerequisitesPipelineBehavior(
        ILogger<PrerequisitesPipelineBehavior<TRequest, TResponse>> logger,
        IEnumerable<IValidator<TRequest>> validators,
        IEnumerable<IAuthorizer<TRequest>> authorizers,
        IMediator mediator,
        ICommandAuthorizationStorage commandAuthorizationStorage)
    {
        _logger = logger;
        _validators = validators;
        _authorizers = authorizers;
        _mediator = mediator;
        _commandAuthorizationStorage = commandAuthorizationStorage;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            _logger.LogDebug(
                "Validating request '{Type}' with {ValidatorCount} authorizers",
                RequestTypeName, _validators.Count());

            var errorsDictionary = PrerequisiteHelpers.GetFailedValidations(
                _validators,
                request);

            if (errorsDictionary.Any())
            {
                throw new ValidationException(errorsDictionary);
            }
        }

        if (_authorizers.Any())
        {
            _logger.LogDebug(
                "Authorizing request '{Type}' with {AuthorizerCount} authorizers",
                RequestTypeName, _authorizers.Count());

            var failedRequirements = await PrerequisiteHelpers.GetFailedAuthorizationRequirementsAsync(
                _logger,
                _mediator,
                _authorizers,
                request,
                RequestTypeName,
                cancellationToken);

            if (failedRequirements.Count > 0)
            {
                try
                {
                    await PrerequisiteHelpers.StoreAuthorizationFailedEvent(
                        _commandAuthorizationStorage,
                        request,
                        RequestTypeName,
                        RequestTypeNamespace,
                        failedRequirements,
                        cancellationToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Unable to store authorization failed event, error: {Error}",
                        exception.Message);
                }

                var combinedMessage = string.Join(". ", failedRequirements.Select(f => f.Message));

                throw new AuthorizationException(combinedMessage);
            }
        }

        return await next();
    }

    private static string RequestTypeName => AuthorizationFailureEventHelpers.GetActionTypeName(typeof(TRequest));
    private static string RequestTypeNamespace => AuthorizationFailureEventHelpers.GetActionTypeNamespace(typeof(TRequest));
}