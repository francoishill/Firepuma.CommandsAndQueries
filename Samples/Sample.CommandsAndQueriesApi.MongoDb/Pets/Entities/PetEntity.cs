using Firepuma.DatabaseRepositories.MongoDb.Abstractions.Entities;
using MongoDB.Driver;

#pragma warning disable CS8618

namespace Sample.CommandsAndQueriesApi.MongoDb.Pets.Entities;

public class PetEntity : BaseMongoDbEntity
{
    public string Type { get; set; }
    public string Name { get; set; }
    public DateTime BornOn { get; set; }
    public DateTime ArrivedOn { get; set; }
    public string SecretLanguage { get; set; }

    public static IEnumerable<CreateIndexModel<PetEntity>> GetSchemaIndexes()
    {
        return new[]
        {
            new CreateIndexModel<PetEntity>(Builders<PetEntity>.IndexKeys.Combine(
                    Builders<PetEntity>.IndexKeys.Ascending(p => p.Type),
                    Builders<PetEntity>.IndexKeys.Ascending(p => p.Name),
                    Builders<PetEntity>.IndexKeys.Ascending(p => p.BornOn)
                ),
                new CreateIndexOptions<PetEntity>
                {
                    Unique = true,
                }),
        };
    }
}