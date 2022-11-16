using System.Reflection;
using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities.Attributes;
using Firepuma.CommandsAndQueries.Abstractions.Entities.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Firepuma.CommandsAndQueries.Abstractions.Entities;

internal static class CommandExecutionEventHelpers
{
    public static string GetTypeName(ICommandRequest commandRequest) => CommandTypeNameHelpers.GetTypeNameExcludingNamespace(commandRequest.GetType());
    public static string GetTypeNamespace(ICommandRequest commandRequest) => CommandTypeNameHelpers.GetTypeNamespace(commandRequest.GetType());
    public static string GetSerializedPayload(ICommandRequest commandRequest) => JsonConvert.SerializeObject(commandRequest, GetCommandPayloadSerializerSettings());

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
}