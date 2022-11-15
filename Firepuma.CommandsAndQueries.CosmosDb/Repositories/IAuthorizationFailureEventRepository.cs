using Firepuma.CommandsAndQueries.CosmosDb.Entities;
using Firepuma.DatabaseRepositories.Abstractions.Repositories;

namespace Firepuma.CommandsAndQueries.CosmosDb.Repositories;

internal interface IAuthorizationFailureEventRepository : IRepository<AuthorizationFailureCosmosDbEvent>
{
}