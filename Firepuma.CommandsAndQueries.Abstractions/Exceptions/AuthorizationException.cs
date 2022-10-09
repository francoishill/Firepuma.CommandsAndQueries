namespace Firepuma.CommandsAndQueries.Abstractions.Exceptions;

public class AuthorizationException : Exception
{
    public AuthorizationException(string message)
        : base(message)
    {
    }
}