using Firepuma.CommandsAndQueries.Abstractions.DomainRequests;

namespace Firepuma.CommandsAndQueries.Abstractions.Commands;

public interface ICommandRequest : IDomainRequest
{
    string CommandId { get; }
    DateTime CreatedOn { get; }
}