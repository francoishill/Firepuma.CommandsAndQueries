using Firepuma.CommandsAndQueries.CosmosDb.Entities;
using Firepuma.CommandsAndQueries.CosmosDb.Services;

namespace Sample.CommandsAndQueriesApi.CosmosDb.Services;

public class AuthorizationFailurePartitionKeyGenerator : IAuthorizationFailurePartitionKeyGenerator
{
    public string GeneratePartitionKey(AuthorizationFailureCosmosDbEvent entity)
    {
        return entity.CreatedOn.ToString("yyyy-MM");
    }
}