using Firepuma.DatabaseRepositories.Abstractions.QuerySpecifications;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.MongoDb.Pets.QuerySpecifications;

public class AllPetsQuerySpecification : QuerySpecification<PetEntity>
{
}