using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.Entities.Attributes;
using MediatR;
using Sample.CommandsAndQueriesApi.Pets.Entities;
using Sample.CommandsAndQueriesApi.Pets.Repositories;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Sample.CommandsAndQueriesApi.Pets.Commands;

public static class CreatePetCommand
{
    public class Payload : BaseCommand<Result>
    {
        public string Type { get; init; }
        public string Name { get; init; }
        public DateTime? BornOn { get; init; }
        public DateTime? ArrivedOn { get; init; }

        [IgnoreCommandAudit]
        public string SecretLanguage { get; init; } // example of ignored property for command audits
    }

    public class Result
    {
        public PetEntity PetEntity { get; init; }
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
                BornOn = payload.BornOn ?? throw new ArgumentNullException($"{nameof(payload.BornOn)} is required"),
                ArrivedOn = payload.ArrivedOn ?? throw new ArgumentNullException($"{nameof(payload.ArrivedOn)} is required"),
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