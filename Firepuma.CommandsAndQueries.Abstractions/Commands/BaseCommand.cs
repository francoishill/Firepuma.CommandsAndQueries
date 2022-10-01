namespace Firepuma.CommandsAndQueries.Abstractions.Commands;

public abstract class BaseCommand<TResponse> : ICommandRequest<TResponse>
{
    public string CommandId { get; } = Guid.NewGuid().ToString();
    public DateTime CreatedOn { get; } = DateTime.UtcNow;
}