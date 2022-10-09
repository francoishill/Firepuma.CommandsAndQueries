using System.Net;

namespace Firepuma.CommandsAndQueries.Abstractions.Exceptions;

public class CommandException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public Error[] Errors { get; set; }

    public CommandException(
        HttpStatusCode statusCode,
        Exception innerException,
        params Error[] errors)
        : base($"Status: {statusCode.ToString()}, Errors: {string.Join(", ", errors.Select(e => $"{e.Code} {e.Message}"))}", innerException)
    {
        StatusCode = statusCode;
        Errors = errors;
    }

    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }
}