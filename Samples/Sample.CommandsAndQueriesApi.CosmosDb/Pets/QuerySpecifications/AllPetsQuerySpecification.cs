using Firepuma.DatabaseRepositories.Abstractions.QuerySpecifications;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.CosmosDb.Pets.QuerySpecifications;

public class AllPetsQuerySpecification : QuerySpecification<PetEntity>
{
}