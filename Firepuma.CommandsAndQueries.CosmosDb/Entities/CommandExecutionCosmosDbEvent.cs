using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.CosmosDb.Entities;

public class CommandExecutionCosmosDbEvent : BaseCommandExecutionEvent
{
    public string PartitionKey { get; set; } = null!;

    public CommandExecutionCosmosDbEvent()
    {
        // used by Azure Cosmos deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public CommandExecutionCosmosDbEvent(ICommandRequest commandRequest)
        : base(commandRequest)
    {
    }
}