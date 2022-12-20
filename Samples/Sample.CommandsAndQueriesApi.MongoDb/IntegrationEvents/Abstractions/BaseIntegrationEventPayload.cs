using MongoDB.Bson;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Abstractions;

public abstract class BaseIntegrationEventPayload
{
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string IntegrationEventId { get; set; } = ObjectId.GenerateNewId().ToString();
}