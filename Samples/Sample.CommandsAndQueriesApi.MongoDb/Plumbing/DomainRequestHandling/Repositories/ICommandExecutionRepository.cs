using Firepuma.DatabaseRepositories.Abstractions.Repositories;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Entities;

namespace Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Repositories;

public interface ICommandExecutionRepository : IRepository<CommandExecutionMongoDbEvent>
{
}