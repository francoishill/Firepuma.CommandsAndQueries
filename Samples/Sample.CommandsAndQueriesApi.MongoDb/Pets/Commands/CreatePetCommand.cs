using System.Text.RegularExpressions;
using Firepuma.CommandsAndQueries.Abstractions.Authorization;
using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities.Attributes;
using FluentValidation;
using MediatR;
using Sample.CommandsAndQueriesApi.MongoDb.AuthorizationRequirements;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Abstractions;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Entities;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Repositories;

#pragma warning disable CS8618

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Sample.CommandsAndQueriesApi.MongoDb.Pets.Commands;

public static class CreatePetCommand
{
    public class Payload : BaseCommand<Result>
    {
        public string Type { get; init; }
        public string Name { get; init; }
        public DateTime BornOn { get; init; }
        public DateTime ArrivedOn { get; init; }

        [IgnoreCommandExecution]
        public string SecretLanguage { get; init; } // example of ignored property for command execution recording
    }

    public class Result : BaseIntegrationEventPayload
    {
        public PetEntity PetEntity { get; init; }
    }

    public sealed class Validator : AbstractValidator<Payload>
    {
        public Validator()
        {
            var allowedTypesPattern = new Regex("Cat|Dog|Fish", RegexOptions.Compiled);
            RuleFor(x => x.Type)
                .Matches(allowedTypesPattern)
                .WithMessage($"Type is not valid, pattern for allowed types is '{allowedTypesPattern}'");

            RuleFor(x => x.BornOn)
                .LessThanOrEqualTo(x => x.ArrivedOn)
                .WithMessage($"{nameof(Payload.BornOn)} must be less than or equal to {nameof(Payload.ArrivedOn)}");
        }
    }

    public class Authorizer : AbstractRequestAuthorizer<Payload>
    {
        public override async Task BuildPolicy(Payload request, CancellationToken cancellationToken)
        {
            UseRequirement(new PetNameMustBeAllowedRequirement(request.Name));
            await Task.CompletedTask;
        }
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
            var newPet = new PetEntity
            {
                Type = payload.Type,
                Name = payload.Name,
                BornOn = payload.BornOn,
                ArrivedOn = payload.ArrivedOn,
                SecretLanguage = payload.SecretLanguage,
            };
            newPet = await _petRepository.AddItemAsync(newPet, cancellationToken);

            return new Result
            {
                PetEntity = newPet,
            };
        }
    }
}