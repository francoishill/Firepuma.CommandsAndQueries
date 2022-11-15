using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.MongoDb.Entities;

public class CommandExecutionMongoDbEvent : BaseCommandExecutionEvent
{
    public CommandExecutionMongoDbEvent()
    {
        // used by Azure Cosmos deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public CommandExecutionMongoDbEvent(ICommandRequest commandRequest)
        : base(commandRequest)
    {
    }
}