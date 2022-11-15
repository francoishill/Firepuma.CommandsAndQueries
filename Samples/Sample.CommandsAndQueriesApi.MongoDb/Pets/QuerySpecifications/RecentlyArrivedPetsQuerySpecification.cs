using Firepuma.DatabaseRepositories.Abstractions.QuerySpecifications;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.MongoDb.Pets.QuerySpecifications;

public class RecentlyArrivedPetsQuerySpecification : QuerySpecification<PetEntity>
{
    public RecentlyArrivedPetsQuerySpecification(DateTime arrivedAfterDate)
    {
        WhereExpressions.Add(pet => pet.ArrivedOn >= arrivedAfterDate);
    }
}