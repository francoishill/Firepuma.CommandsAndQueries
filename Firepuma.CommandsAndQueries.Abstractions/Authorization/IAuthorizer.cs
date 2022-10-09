namespace Firepuma.CommandsAndQueries.Abstractions.Authorization;

public interface IAuthorizer<in T>
{
    IEnumerable<IAuthorizationRequirement> Requirements { get; }
    Task BuildPolicy(T instance, CancellationToken cancellationToken);
}