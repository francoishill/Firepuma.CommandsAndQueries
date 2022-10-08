using Firepuma.CommandsAndQueries.CosmosDb;
using Firepuma.DatabaseRepositories.CosmosDb;
using MediatR;
using Sample.CommandsAndQueriesApi.Configuration;
using Sample.CommandsAndQueriesApi.Pets.Controllers;
using Sample.CommandsAndQueriesApi.Pets.Entities;
using Sample.CommandsAndQueriesApi.Pets.Repositories;
using Sample.CommandsAndQueriesApi.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddCosmosDbRepositories(options =>
    {
        options.ConnectionString = configuration.GetValue<string>("CosmosDb:ConnectionString");
        options.DatabaseId = configuration.GetValue<string>("CosmosDb:DatabaseId");
    });
builder.Services
    .AddCosmosDbRepository<
        PetEntity,
        IPetRepository,
        PetCosmosDbRepository>(
        CosmosContainers.Pets.ContainerProperties.Id,
        (
            logger,
            container,
            _) => new PetCosmosDbRepository(logger, container));
builder.Services
    .AddCommandsAuditWithCosmosDbPipelineBehavior<
        CommandAuditPartitionKeyGenerator>(
        CosmosContainers.CommandExecutions.ContainerProperties.Id);
builder.Services.AddMediatR(typeof(PetsController));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();