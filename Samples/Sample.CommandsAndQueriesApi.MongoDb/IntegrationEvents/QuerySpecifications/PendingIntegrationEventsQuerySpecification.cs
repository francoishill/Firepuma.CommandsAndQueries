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
            c.ExtraValues.ContainsKey(ExtraValuesKeys.PAYLOAD_JSON)
            && !c.ExtraValues.ContainsKey(ExtraValuesKeys.PUBLISH_RESULT_TIME)
            && (
                c.ExtraValues[ExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS] == null
                || (int?)c.ExtraValues[ExtraValuesKeys.LOCK_UNTIL_UNIX_SECONDS] < nowUnixSeconds));
    }
}