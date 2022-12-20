using Firepuma.CommandsAndQueries.MongoDb.Entities;
using Firepuma.DatabaseRepositories.Abstractions.Repositories;

namespace Firepuma.CommandsAndQueries.MongoDb.Repositories;

public interface ICommandExecutionRepository : IRepository<CommandExecutionMongoDbEvent>
{
}