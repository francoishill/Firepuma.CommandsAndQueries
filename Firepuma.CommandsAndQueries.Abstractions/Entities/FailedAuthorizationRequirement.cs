using Firepuma.CommandsAndQueries.Abstractions.Authorization;
using Firepuma.CommandsAndQueries.Abstractions.Entities.Helpers;

namespace Firepuma.CommandsAndQueries.Abstractions.Entities;

#pragma warning disable CS8618

public class FailedAuthorizationRequirement
{
    public string TypeName { get; set; }
    public string TypeNamespace { get; set; }
    public object Payload { get; set; }
    public string Message { get; set; }

    // ReSharper disable once UnusedMember.Global
    public FailedAuthorizationRequirement()
    {
        // used by Azure Cosmos deserialization (including the Add methods, like repository.AddItemAsync)
    }

    public FailedAuthorizationRequirement(IAuthorizationRequirement requirement, string message)
    {
        var type = requirement.GetType();
        TypeName = CommandTypeNameHelpers.GetTypeNameExcludingNamespace(type);
        TypeNamespace = CommandTypeNameHelpers.GetTypeNamespace(type);
        Payload = requirement;
        Message = message;
    }
}