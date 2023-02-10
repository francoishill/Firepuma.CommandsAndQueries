using Firepuma.CommandsAndQueries.Abstractions.DomainRequests;
using MediatR;

namespace Firepuma.CommandsAndQueries.Abstractions.Queries;

public interface IQueryRequest : IDomainRequest
{
}

public interface IQueryRequest<out TResponse> : IRequest<TResponse>, IQueryRequest
{
}