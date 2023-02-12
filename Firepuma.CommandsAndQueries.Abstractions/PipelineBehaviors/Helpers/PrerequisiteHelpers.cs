using Firepuma.CommandsAndQueries.Abstractions.Authorization;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors.Helpers;

public static class PrerequisiteHelpers
{
    public static async Task<List<FailedAuthorizationRequirement>> GetFailedAuthorizationRequirementsAsync<TRequest>(
        ILogger logger,
        ISender mediator,
        IEnumerable<IAuthorizer<TRequest>> authorizers,
        TRequest request,
        string requestTypeName,
        CancellationToken cancellationToken)
    {
        var requirements = new List<IAuthorizationRequirement>();

        foreach (var authorizer in authorizers)
        {
            await authorizer.BuildPolicy(request, cancellationToken);
            requirements.AddRange(authorizer.Requirements);
        }

        logger.LogDebug(
            "Request '{Type}' has {Count} authorization requirements", requestTypeName,
            requirements.Count);

        var failedRequirements = new List<FailedAuthorizationRequirement>();

        foreach (var requirement in requirements.Distinct())
        {
            var result = await mediator.Send(requirement, cancellationToken);

            if (!result.IsAuthorized)
            {
                logger.LogDebug(
                    "Requirement '{Requirement}' is not met for request type '{Type}', failure message: '{Message}'",
                    requirement.GetType().FullName, requestTypeName, result.FailureMessage);

                failedRequirements.Add(new FailedAuthorizationRequirement(
                    requirement,
                    result.FailureMessage ?? "[UNKNOWN FAILURE]"));
            }
            else
            {
                logger.LogDebug(
                    "Requirement '{Requirement}' successfully passed for request type '{Type}'",
                    requirement.GetType().FullName, requestTypeName);
            }
        }

        return failedRequirements;
    }

    public static List<ValidationFailure> GetFailedValidations<TRequest>(
        IEnumerable<IValidator<TRequest>> validators,
        TRequest request)
    {
        var context = new ValidationContext<TRequest>(request);
        var errorsDictionary = validators
            .Select(x => x.Validate(context))
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .ToList();
        return errorsDictionary;
    }

    public static void PopulateAuthorizationFailedEvent<TRequest>(
        IAuthorizationFailureEvent authorizationFailureEvent,
        TRequest request,
        string requestTypeName,
        string requestTypeNamespace,
        List<FailedAuthorizationRequirement> failedRequirements)
    {
        authorizationFailureEvent.CreatedOn = DateTime.UtcNow;

        authorizationFailureEvent.ActionTypeName = requestTypeName;
        authorizationFailureEvent.ActionTypeNamespace = requestTypeNamespace;
        authorizationFailureEvent.ActionPayload = request;
        authorizationFailureEvent.FailedRequirements = failedRequirements.ToArray();
    }
}