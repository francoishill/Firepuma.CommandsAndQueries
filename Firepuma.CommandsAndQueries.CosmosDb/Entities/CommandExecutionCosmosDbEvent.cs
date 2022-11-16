using System.Diagnostics;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.DatabaseRepositories.CosmosDb.Abstractions.Entities;

#pragma warning disable CS8618
// ReSharper disable EmptyConstructor

namespace Firepuma.CommandsAndQueries.CosmosDb.Entities;

[DebuggerDisplay("{ToString()}")]
public class CommandExecutionCosmosDbEvent : BaseCosmosDbEntity, ICommandExecutionEvent
{
    public string PartitionKey { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public string CommandId { get; set; }
    public string TypeName { get; set; }
    public string TypeNamespace { get; set; }
    public string Payload { get; set; }

    public bool? Successful { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorStackTrack { get; set; }
    public double ExecutionTimeInSeconds { get; set; }
    public double TotalTimeInSeconds { get; set; }

    public CommandExecutionCosmosDbEvent()
    {
        // used by Azure Cosmos deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public override string ToString()
    {
        return $"{Id}/{CommandId}/{TypeNamespace}.{TypeName}";
    }
}