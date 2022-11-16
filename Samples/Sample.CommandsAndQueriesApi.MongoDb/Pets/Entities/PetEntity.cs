using Firepuma.DatabaseRepositories.MongoDb.Abstractions.Entities;

#pragma warning disable CS8618

namespace Sample.CommandsAndQueriesApi.MongoDb.Pets.Entities;

public class PetEntity : BaseMongoDbEntity
{
    public string Type { get; set; }
    public string Name { get; set; }
    public DateTime BornOn { get; set; }
    public DateTime ArrivedOn { get; set; }
    public string SecretLanguage { get; set; }
}