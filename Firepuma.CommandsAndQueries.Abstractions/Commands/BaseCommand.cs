namespace Firepuma.CommandsAndQueries.Abstractions.Commands;

public abstract class BaseCommand : ICommandRequest
{
    public string CommandId { get; } = Guid.NewGuid().ToString();
    public DateTime CreatedOn { get; } = DateTime.UtcNow;
}

public abstract class BaseCommand<TResponse> : BaseCommand, ICommandRequest<TResponse>
{
}