using Firepuma.CommandsAndQueries.Abstractions.Entities.Helpers;

namespace Firepuma.CommandsAndQueries.Abstractions.Entities;

public static class AuthorizationFailureEventHelpers
{
    public static string GetActionTypeName(Type actionType) => CommandTypeNameHelpers.GetTypeNameExcludingNamespace(actionType);
    public static string GetActionTypeNamespace(Type actionType) => CommandTypeNameHelpers.GetTypeNamespace(actionType);
}