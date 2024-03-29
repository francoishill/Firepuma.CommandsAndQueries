﻿using Firepuma.CommandsAndQueries.Abstractions.Queries;
using Firepuma.DatabaseRepositories.Abstractions.QuerySpecifications;
using FluentValidation;
using MediatR;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Entities;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.QuerySpecifications;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Repositories;

#pragma warning disable CS8618

namespace Sample.CommandsAndQueriesApi.MongoDb.Pets.Queries;

public static class GetPetsQuery
{
    public class Payload : BaseQuery<Result>
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

    // ReSharper disable once UnusedType.Global
    public class Validator : AbstractValidator<Payload>
    {
        public Validator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.FilterArrivedAfterDate)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Must be before NOW");
        }
    }
}