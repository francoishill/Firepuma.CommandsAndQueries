using Firepuma.DatabaseRepositories.Abstractions.Repositories;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.MongoDb.Pets.Repositories;

public interface IPetRepository : IRepository<PetEntity>
{
}