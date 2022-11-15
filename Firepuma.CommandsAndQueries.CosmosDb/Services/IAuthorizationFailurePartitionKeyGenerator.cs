using Firepuma.CommandsAndQueries.CosmosDb.Entities;

namespace Firepuma.CommandsAndQueries.CosmosDb.Services;

public interface IAuthorizationFailurePartitionKeyGenerator
{
    string GeneratePartitionKey(AuthorizationFailureCosmosDbEvent entity);
}