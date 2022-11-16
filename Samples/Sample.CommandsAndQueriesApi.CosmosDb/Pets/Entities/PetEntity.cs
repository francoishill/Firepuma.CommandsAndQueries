using Firepuma.DatabaseRepositories.CosmosDb.Abstractions.Entities;

#pragma warning disable CS8618

namespace Sample.CommandsAndQueriesApi.CosmosDb.Pets.Entities;

public class PetEntity : BaseCosmosDbEntity
{
    public string Type { get; set; }
    public string Name { get; set; }
    public DateTime BornOn { get; set; }
    public DateTime ArrivedOn { get; set; }
    public string SecretLanguage { get; set; }
}