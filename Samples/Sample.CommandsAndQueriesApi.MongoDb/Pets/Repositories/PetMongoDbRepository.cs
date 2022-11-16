using Firepuma.DatabaseRepositories.MongoDb.Repositories;
using MongoDB.Driver;
using Sample.CommandsAndQueriesApi.MongoDb.Pets.Entities;

namespace Sample.CommandsAndQueriesApi.MongoDb.Pets.Repositories;

public class PetMongoDbRepository : MongoDbRepository<PetEntity>, IPetRepository
{
    public PetMongoDbRepository(ILogger logger, IMongoCollection<PetEntity> collection)
        : base(logger, collection)
    {
    }
}