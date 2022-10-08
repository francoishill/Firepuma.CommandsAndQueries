using MediatR;

namespace Firepuma.CommandsAndQueries.Abstractions.Queries;

public interface IQueryRequest
{
}

public interface IQueryRequest<out TResponse> : IRequest<TResponse>, IQueryRequest
{
}