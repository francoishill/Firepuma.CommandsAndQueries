using Firepuma.CommandsAndQueries.MongoDb.Entities;
using Firepuma.DatabaseRepositories.Abstractions.QuerySpecifications;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Constants;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.QuerySpecifications;

internal class PendingIntegrationEventsQuerySpecification : QuerySpecification<CommandExecutionMongoDbEvent>
{
    public PendingIntegrationEventsQuerySpecification()
    {
        var nowUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        WhereExpressions.Add(c =>
            c.Successful == true
            && c.ExtraValues.ContainsKey(IntegrationEventExtraValuesKeys.PAYLOAD_JSON)
            && !c.ExtraValues.ContainsKey(IntegrationEventExtraValuesKeys.PUBLISH_RESULT_TIME)
            && (
                c.ExtraValues[IntegrationEventExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS] == null
                || (int?)c.ExtraValues[IntegrationEventExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS] < nowUnixSeconds));
    }
}