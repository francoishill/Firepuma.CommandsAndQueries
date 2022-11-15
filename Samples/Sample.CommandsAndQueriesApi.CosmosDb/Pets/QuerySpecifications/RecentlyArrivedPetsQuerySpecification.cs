using Firepuma.DatabaseRepositories.Abstractions.QuerySpecifications;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.CosmosDb.Pets.QuerySpecifications;

public class RecentlyArrivedPetsQuerySpecification : QuerySpecification<PetEntity>
{
    public RecentlyArrivedPetsQuerySpecification(DateTime arrivedAfterDate)
    {
        WhereExpressions.Add(pet => pet.ArrivedOn >= arrivedAfterDate);
    }
}