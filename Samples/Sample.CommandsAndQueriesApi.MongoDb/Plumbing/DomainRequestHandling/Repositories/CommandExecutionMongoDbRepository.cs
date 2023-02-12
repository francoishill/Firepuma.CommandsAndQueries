using Firepuma.DatabaseRepositories.MongoDb.Repositories;
using MongoDB.Driver;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Entities;

namespace Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Repositories;

internal class CommandExecutionMongoDbRepository : MongoDbRepository<CommandExecutionMongoDbEvent>, ICommandExecutionRepository
{
    public CommandExecutionMongoDbRepository(
        ILogger logger,
        IMongoCollection<CommandExecutionMongoDbEvent> collection)
        : base(logger, collection)
    {
    }
}