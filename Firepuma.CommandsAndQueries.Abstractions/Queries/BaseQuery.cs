using MediatR;

namespace Firepuma.CommandsAndQueries.Abstractions.Queries;

public abstract class BaseQuery : IQueryRequest, IRequest
{
}

public abstract class BaseQuery<TResponse> : IQueryRequest, IRequest<TResponse>
{
}