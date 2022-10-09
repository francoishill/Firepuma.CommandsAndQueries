// ReSharper disable RedundantAnonymousTypePropertyName

using Firepuma.CommandsAndQueries.Abstractions.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Sample.CommandsAndQueriesApi.HttpResponses;

internal static class HttpResponseFactory
{
    public static ActionResult CreateResponseMessageResult(this CommandException commandException)
    {
        return new JsonResult(new
        {
            Errors = commandException.Errors,
        })
        {
            StatusCode = (int)commandException.StatusCode,
        };
    }
}