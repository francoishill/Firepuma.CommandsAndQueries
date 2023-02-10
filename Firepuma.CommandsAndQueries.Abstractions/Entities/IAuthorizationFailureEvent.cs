// ReSharper disable MemberCanBePrivate.Global

#pragma warning disable CS8618

namespace Firepuma.CommandsAndQueries.Abstractions.Entities;

public interface IAuthorizationFailureEvent
{
    DateTime CreatedOn { get; set; }

    string ActionTypeName { get; set; }
    string ActionTypeNamespace { get; set; }
    object? ActionPayload { get; set; }
    FailedAuthorizationRequirement[] FailedRequirements { get; set; }
}