using MediatR;

namespace Firepuma.CommandsAndQueries.Abstractions;

public interface IQueryRequest
{
}

public interface IQueryRequest<out TResponse> : IRequest<TResponse>, IQueryRequest
{
}