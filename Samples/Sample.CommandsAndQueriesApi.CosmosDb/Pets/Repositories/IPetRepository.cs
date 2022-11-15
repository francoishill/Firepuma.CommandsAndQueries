using Firepuma.DatabaseRepositories.Abstractions.Repositories;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.CosmosDb.Pets.Repositories;

public interface IPetRepository : IRepository<PetEntity>
{
}