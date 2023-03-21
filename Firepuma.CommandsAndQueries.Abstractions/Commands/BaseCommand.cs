using MediatR;

// ReSharper disable UnusedType.Global

namespace Firepuma.CommandsAndQueries.Abstractions.Commands;

public abstract class BaseCommand : ICommandRequest, IRequest
{
    public string CommandId { get; } = Guid.NewGuid().ToString();
    public DateTime CreatedOn { get; } = DateTime.UtcNow;
}

public abstract class BaseCommand<TResponse> : ICommandRequest, IRequest<TResponse>
{
    public string CommandId { get; } = Guid.NewGuid().ToString();
    public DateTime CreatedOn { get; } = DateTime.UtcNow;
}