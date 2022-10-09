using Firepuma.CommandsAndQueries.Abstractions.Entities;

namespace Firepuma.CommandsAndQueries.CosmosDb.Services;

public interface IAuthorizationFailurePartitionKeyGenerator
{
    string GeneratePartitionKey(AuthorizationFailureEvent entity);
}