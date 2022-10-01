using Firepuma.DatabaseRepositories.Abstractions.Repositories;
using Sample.CommandsAndQueriesApi.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.Pets.Repositories;

public interface IPetRepository : IRepository<PetEntity>
{
}