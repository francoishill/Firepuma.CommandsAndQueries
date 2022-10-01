using Firepuma.DatabaseRepositories.Abstractions.QuerySpecifications;
using Sample.CommandsAndQueriesApi.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.Pets.QuerySpecifications;

public class RecentlyArrivedPetsQuerySpecification : QuerySpecification<PetEntity>
{
    public RecentlyArrivedPetsQuerySpecification(DateTime arrivedAfterDate)
    {
        WhereExpressions.Add(pet => pet.ArrivedOn >= arrivedAfterDate);
    }
}