using System.Diagnostics;
using System.Reflection;
using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities.Attributes;
using Firepuma.CommandsAndQueries.Abstractions.Entities.Helpers;
using Firepuma.DatabaseRepositories.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#pragma warning disable CS8618

namespace Firepuma.CommandsAndQueries.Abstractions.Entities;

[DebuggerDisplay("{ToString()}")]
public class CommandExecutionEvent : BaseEntity
{
    public string PartitionKey { get; set; }
    public string CommandId { get; set; }
    public string TypeName { get; set; }
    public string TypeNamespace { get; set; }
    public string Payload { get; set; }
    public DateTime CreatedOn { get; set; }

    public bool? Successful { get; set; }
    public string? Result { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorStackTrack { get; set; }
    public double ExecutionTimeInSeconds { get; set; }
    public double TotalTimeInSeconds { get; set; }

    // ReSharper disable once UnusedMember.Global, EmptyConstructor 
    public CommandExecutionEvent()
    {
        // used by Azure Cosmos deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public CommandExecutionEvent(ICommandRequest commandRequest)
    {
        CommandId = commandRequest.CommandId;
        TypeName = CommandTypeNameHelpers.GetTypeNameExcludingNamespace(commandRequest.GetType());
        TypeNamespace = CommandTypeNameHelpers.GetTypeNamespace(commandRequest.GetType());
        Payload = JsonConvert.SerializeObject(commandRequest, GetCommandPayloadSerializerSettings());
        CreatedOn = commandRequest.CreatedOn;
    }

    private static JsonSerializerSettings GetCommandPayloadSerializerSettings()
    {
        var jsonSerializerSettings = new JsonSerializerSettings();
        jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        jsonSerializerSettings.ContractResolver = new JsonIgnoreCommandExecutionPropertiesResolver();
        return jsonSerializerSettings;
    }

    private class JsonIgnoreCommandExecutionPropertiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            var ignoreProperty = (IgnoreCommandExecutionAttribute?)property.AttributeProvider?.GetAttributes(typeof(IgnoreCommandExecutionAttribute), true).FirstOrDefault();

            if (ignoreProperty != null)
            {
                property.Ignored = true;
            }

            return property;
        }
    }

    public override string ToString()
    {
        return $"{Id}/{CommandId}/{TypeNamespace}.{TypeName}";
    }
}