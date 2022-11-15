using Firepuma.CommandsAndQueries.MongoDb.Entities;
using Firepuma.DatabaseRepositories.MongoDb.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Firepuma.CommandsAndQueries.MongoDb.Repositories;

internal class AuthorizationFailureEventMongoDbRepository : MongoDbRepository<AuthorizationFailureMongoDbEvent>, IAuthorizationFailureEventRepository
{
    public AuthorizationFailureEventMongoDbRepository(
        ILogger logger,
        IMongoCollection<AuthorizationFailureMongoDbEvent> collection)
        : base(logger, collection)
    {
    }

    protected override string GenerateId(AuthorizationFailureMongoDbEvent entity)
    {
        return ObjectId.GenerateNewId().ToString();
    }
}