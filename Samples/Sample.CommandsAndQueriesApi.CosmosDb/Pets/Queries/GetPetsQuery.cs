using Firepuma.CommandsAndQueries.Abstractions.Queries;
using Firepuma.DatabaseRepositories.Abstractions.QuerySpecifications;
using MediatR;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.Entities;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.QuerySpecifications;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.Repositories;

#pragma warning disable CS8618

namespace Sample.CommandsAndQueriesApi.CosmosDb.Pets.Queries;

public static class GetPetsQuery
{
    public class Payload : IQueryRequest<Result>
    {
        public DateTime? FilterArrivedAfterDate { get; set; }
    }

    public class Result
    {
        public PetEntity[] Pets { get; set; }
    }

    public class Handler : IRequestHandler<Payload, Result>
    {
        private readonly IPetRepository _petRepository;

        public Handler(
            IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        public async Task<Result> Handle(
            Payload payload,
            CancellationToken cancellationToken)
        {
            IQuerySpecification<PetEntity> querySpecification =
                payload.FilterArrivedAfterDate != null
                    ? new RecentlyArrivedPetsQuerySpecification(payload.FilterArrivedAfterDate.Value)
                    : new AllPetsQuerySpecification();

            var pets = await _petRepository.GetItemsAsync(querySpecification, cancellationToken);

            return new Result
            {
                Pets = pets.ToArray(),
            };
        }
    }
}