using Firepuma.CommandsAndQueries.Abstractions.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sample.CommandsAndQueriesApi.HttpResponses;
using Sample.CommandsAndQueriesApi.Pets.Commands;
using Sample.CommandsAndQueriesApi.Pets.Controllers.Requests;
using Sample.CommandsAndQueriesApi.Pets.Entities;
using Sample.CommandsAndQueriesApi.Pets.Queries;

// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

namespace Sample.CommandsAndQueriesApi.Pets.Controllers;

[ApiController]
[Route("[controller]")]
public class PetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PetsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<PetEntity>> AddPet(AddPetRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var createPetCommand = new CreatePetCommand.Payload
            {
                Type = request.Type,
                Name = request.Name,
                BornOn = request.BornOn ?? throw new ArgumentNullException($"{nameof(request.BornOn)} is required"),
                ArrivedOn = request.ArrivedOn ?? throw new ArgumentNullException($"{nameof(request.ArrivedOn)} is required"),
                SecretLanguage = Guid.NewGuid().ToString("X"),
            };

            var createResult = await _mediator.Send(createPetCommand, cancellationToken);

            return createResult.PetEntity;
        }
        catch (CommandException commandException)
        {
            return commandException.CreateResponseMessageResult();
        }
    }

    [HttpGet]
    public async Task<IEnumerable<PetEntity>> GetAllPets(CancellationToken cancellationToken)
    {
        var query = new GetPetsQuery.Payload
        {
            FilterArrivedAfterDate = null,
        };

        var result = await _mediator.Send(query, cancellationToken);

        //TODO: map result to Api DTO objects
        return result.Pets;
    }

    [HttpGet("recently-arrived")]
    public async Task<IEnumerable<PetEntity>> GetAllRecentlyArrivedPets(CancellationToken cancellationToken)
    {
        var query = new GetPetsQuery.Payload
        {
            FilterArrivedAfterDate = DateTime.UtcNow.AddMonths(-1),
        };

        var result = await _mediator.Send(query, cancellationToken);

        //TODO: map result to Api DTO objects
        return result.Pets;
    }
}