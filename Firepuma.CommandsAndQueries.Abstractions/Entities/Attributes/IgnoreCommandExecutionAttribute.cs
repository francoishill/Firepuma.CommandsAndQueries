namespace Firepuma.CommandsAndQueries.Abstractions.Entities.Attributes;

/// <summary>
/// Use this attribute on a command property to ignore it, using it on secret properties is a good example.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true)]
public class IgnoreCommandExecutionAttribute : Attribute
{
}