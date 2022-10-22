// ReSharper disable ReplaceSubstringWithRangeIndexer

namespace Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors.Helpers;

internal static class BehaviorPipelineHelpers
{
    public static string? GetShortTypeName(Type type)
    {
        var fullName = type.FullName;
        var lastDotIndex = fullName?.LastIndexOf(".");

        return lastDotIndex >= 0
            ? fullName?.Substring(lastDotIndex.Value + 1)
            : fullName;
    }
}