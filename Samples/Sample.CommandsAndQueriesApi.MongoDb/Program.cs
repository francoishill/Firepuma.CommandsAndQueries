using Firepuma.DatabaseRepositories.MongoDb;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver.Core.Events;
using Sample.CommandsAndQueriesApi.MongoDb.Configuration;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Commands;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Entities;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Repositories;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling;

// ReSharper disable ArgumentsStyleNamedExpression

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoDbConfigSection = builder.Configuration.GetSection("MongoDb");
builder.Services.AddOptions<MongoDbOptions>().Bind(mongoDbConfigSection).ValidateDataAnnotations().ValidateOnStart();
var mongoDbOptions = mongoDbConfigSection.Get<MongoDbOptions>()!;

builder.Services
    .AddMongoDbRepositories(options =>
        {
            options.ConnectionString = mongoDbOptions.ConnectionString;
            options.DatabaseName = mongoDbOptions.DatabaseName;
        },
        configureClusterBuilder: cb =>
        {
            if (builder.Environment.IsDevelopment())
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    Console.WriteLine($"MongoCommand: {e.CommandName} - {e.Command.ToJson()}");
                });
            }
        });
builder.Services
    .AddMongoDbRepository<
        PetEntity,
        IPetRepository,
        PetMongoDbRepository>(
        mongoDbOptions.PetsCollectionName,
        (logger, collection, _) => new PetMongoDbRepository(logger, collection),
        indexesFactory: PetEntity.GetSchemaIndexes);

var assembliesWithCommandHandlers = new[]
{
    typeof(CreatePetCommand).Assembly,
}.Distinct().ToArray();
builder.Services.AddCommandHandlingMediatRStorageAndPipelines(mongoDbOptions, assembliesWithCommandHandlers);

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