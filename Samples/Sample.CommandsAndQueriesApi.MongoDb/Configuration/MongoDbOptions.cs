using System.ComponentModel.DataAnnotations;

namespace Sample.CommandsAndQueriesApi.MongoDb.Configuration;

public class MongoDbOptions
{
    [Required]
    public string ConnectionString { get; set; } = null!;

    [Required]
    public string DatabaseName { get; set; } = null!;

    [Required]
    public string PetsCollectionName { get; set; } = null!;

    [Required]
    public string AuthorizationFailuresCollectionName { get; set; } = null!;

    [Required]
    public string CommandExecutionsCollectionName { get; set; } = null!;
}