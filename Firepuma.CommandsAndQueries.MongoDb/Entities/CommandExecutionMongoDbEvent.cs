using System.Diagnostics;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.DatabaseRepositories.MongoDb.Abstractions.Entities;
using MongoDB.Driver;

#pragma warning disable CS8618
// ReSharper disable EmptyConstructor

namespace Firepuma.CommandsAndQueries.MongoDb.Entities;

[DebuggerDisplay("{ToString()}")]
public class CommandExecutionMongoDbEvent : BaseMongoDbEntity, ICommandExecutionEvent
{
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

    public Dictionary<string, object?> ExtraValues { get; set; } = new();

    public CommandExecutionMongoDbEvent()
    {
        // used by Mongo deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public override string ToString()
    {
        return $"{Id}/{CommandId}/{TypeNamespace}.{TypeName}";
    }

    public static IEnumerable<CreateIndexModel<CommandExecutionMongoDbEvent>> GetSchemaIndexes()
    {
        return Array.Empty<CreateIndexModel<CommandExecutionMongoDbEvent>>();
    }
}