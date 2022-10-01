using MediatR;

namespace Firepuma.CommandsAndQueries.Abstractions.Commands;

public interface ICommandRequest
{
    string CommandId { get; }
    DateTime CreatedOn { get; }
}

public interface ICommandRequest<out TResponse> : IRequest<TResponse>, ICommandRequest
{
}