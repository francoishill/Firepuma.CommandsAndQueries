namespace Firepuma.CommandsAndQueries.Abstractions.Exceptions;

public class PreconditionFailedException : Exception
{
    public PreconditionFailedException(string message)
        : base(message)
    {
    }
}