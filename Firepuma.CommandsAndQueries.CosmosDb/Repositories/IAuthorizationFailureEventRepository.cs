using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.DatabaseRepositories.Abstractions.Repositories;

namespace Firepuma.CommandsAndQueries.CosmosDb.Repositories;

public interface IAuthorizationFailureEventRepository : IRepository<AuthorizationFailureEvent>
{
}