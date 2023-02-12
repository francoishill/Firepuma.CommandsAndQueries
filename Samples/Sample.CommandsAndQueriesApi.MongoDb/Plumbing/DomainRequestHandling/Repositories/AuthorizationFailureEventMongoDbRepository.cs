using Firepuma.DatabaseRepositories.MongoDb.Repositories;
using MongoDB.Driver;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Entities;

namespace Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Repositories;

internal class AuthorizationFailureEventMongoDbRepository : MongoDbRepository<AuthorizationFailureMongoDbEvent>, IAuthorizationFailureEventRepository
{
    public AuthorizationFailureEventMongoDbRepository(
        ILogger logger,
        IMongoCollection<AuthorizationFailureMongoDbEvent> collection)
        : base(logger, collection)
    {
    }
}