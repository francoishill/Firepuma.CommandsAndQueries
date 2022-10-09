using MediatR;

namespace Firepuma.CommandsAndQueries.Abstractions.Authorization
{
    public interface IAuthorizationHandler<TRequest> : IRequestHandler<TRequest, AuthorizationResult>
        where TRequest : IRequest<AuthorizationResult>
    {
    }
}