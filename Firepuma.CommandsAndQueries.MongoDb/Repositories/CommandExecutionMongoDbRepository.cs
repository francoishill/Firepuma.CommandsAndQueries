using Firepuma.CommandsAndQueries.MongoDb.Entities;
using Firepuma.DatabaseRepositories.MongoDb.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Firepuma.CommandsAndQueries.MongoDb.Repositories;

internal class CommandExecutionMongoDbRepository : MongoDbRepository<CommandExecutionMongoDbEvent>, ICommandExecutionRepository
{
    public CommandExecutionMongoDbRepository(
        ILogger logger,
        IMongoCollection<CommandExecutionMongoDbEvent> collection)
        : base(logger, collection)
    {
    }
}