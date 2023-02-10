using Firepuma.CommandsAndQueries.Abstractions.DomainRequests;
using MediatR;

namespace Firepuma.CommandsAndQueries.Abstractions.Commands;

public interface ICommandRequest : IDomainRequest
{
    string CommandId { get; }
    DateTime CreatedOn { get; }
}

public interface ICommandRequest<out TResponse> : IRequest<TResponse>, ICommandRequest
{
}