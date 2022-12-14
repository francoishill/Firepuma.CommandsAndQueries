using Firepuma.CommandsAndQueries.CosmosDb;
using Firepuma.CommandsAndQueries.CosmosDb.Config;
using Firepuma.DatabaseRepositories.CosmosDb;
using MediatR;
using Sample.CommandsAndQueriesApi.CosmosDb.Configuration;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.Commands;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.Entities;
using Sample.CommandsAndQueriesApi.CosmosDb.Pets.Repositories;
using Sample.CommandsAndQueriesApi.CosmosDb.Services;

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
        (logger, container, _) => new PetCosmosDbRepository(logger, container));

var assembliesWithCommandHandlers = new[]
{
    typeof(CreatePetCommand).Assembly,
}.Distinct().ToArray();
builder.Services
    .AddCommandHandlingWithCosmosDbStorage(
        new CosmosDbCommandHandlingOptions
        {
            AddWrapCommandExceptionsPipelineBehavior = true,
            AddLoggingScopePipelineBehavior = true,
            AddPerformanceLoggingPipelineBehavior = true,

            AddValidationBehaviorPipeline = true,
            ValidationHandlerMarkerAssemblies = assembliesWithCommandHandlers,

            AddAuthorizationBehaviorPipeline = true,
            AuthorizationFailurePartitionKeyGenerator = typeof(AuthorizationFailurePartitionKeyGenerator),
            AuthorizationFailureEventContainerName = CosmosContainers.AuthorizationFailures.ContainerProperties.Id,
            AuthorizationHandlerMarkerAssemblies = assembliesWithCommandHandlers,

            AddRecordingOfExecution = true,
            CommandExecutionPartitionKeyGenerator = typeof(CommandExecutionPartitionKeyGenerator),
            CommandExecutionEventContainerName = CosmosContainers.CommandExecutions.ContainerProperties.Id,
        });
builder.Services.AddMediatR(assembliesWithCommandHandlers);

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