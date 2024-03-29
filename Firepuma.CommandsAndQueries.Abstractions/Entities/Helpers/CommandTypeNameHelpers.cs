﻿using System.Collections.Concurrent;

namespace Firepuma.CommandsAndQueries.Abstractions.Entities.Helpers;

internal static class CommandTypeNameHelpers
{
    private static readonly ConcurrentDictionary<Type, string> _cache = new ConcurrentDictionary<Type, string>();

    public static string GetTypeNameExcludingNamespace(Type type)
    {
        return _cache.GetOrAdd(type, TypeNameExcludingNamespace);
    }

    public static string GetTypeNamespace(Type type)
    {
        return type.Namespace ?? "NO_NAMESPACE";
    }

    private static string TypeNameExcludingNamespace(Type type)
    {
        if (type.FullName == null) return "[NULL_FULLNAME]";
        if (type.Namespace == null) return "[NULL_NAMESPACE]";

        return type.FullName.Substring(type.Namespace.Length + 1);
    }
}