using Firepuma.CommandsAndQueries.CosmosDb.Entities;
using Firepuma.CommandsAndQueries.CosmosDb.Services;
using Firepuma.DatabaseRepositories.CosmosDb.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Firepuma.CommandsAndQueries.CosmosDb.Repositories;

internal class CommandExecutionCosmosDbRepository : CosmosDbRepository<CommandExecutionCosmosDbEvent>, ICommandExecutionRepository
{
    private readonly ICommandExecutionPartitionKeyGenerator _partitionKeyGenerator;

    public CommandExecutionCosmosDbRepository(
        ILogger logger,
        Container container,
        IServiceProvider serviceProvider)
        : base(logger, container)
    {
        _partitionKeyGenerator = serviceProvider.GetRequiredService<ICommandExecutionPartitionKeyGenerator>();
    }

    protected override string GenerateId(CommandExecutionCosmosDbEvent entity)
    {
        var partitionKey = _partitionKeyGenerator.GeneratePartitionKey(entity);
        entity.PartitionKey = partitionKey;
        return $"{Guid.NewGuid().ToString()}:{partitionKey}";
    }

    protected override PartitionKey ResolvePartitionKey(string entityId) => new(entityId.Split(':')[1]);
}