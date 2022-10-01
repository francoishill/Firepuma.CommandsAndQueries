using Firepuma.CommandsAndQueries.Abstractions;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.CosmosDb.Repositories;
using Firepuma.CommandsAndQueries.CosmosDb.Services;
using Firepuma.DatabaseRepositories.CosmosDb;
using Microsoft.Extensions.DependencyInjection;

namespace Firepuma.CommandsAndQueries.CosmosDb;

public static class ServiceCollectionExtensions
{
    public static void AddCommandsAuditWithCosmosDbPipelineBehavior<TPartitionKeyGenerator>(
        this IServiceCollection services,
        string commandExecutionEventContainerName)
        where TPartitionKeyGenerator : class, ICommandAuditPartitionKeyGenerator
    {
        services.AddSingleton<ICommandAuditPartitionKeyGenerator, TPartitionKeyGenerator>();

        services.AddCosmosDbRepository<
            CommandExecutionEvent,
            ICommandExecutionRepository,
            CommandExecutionCosmosDbRepository>(
            commandExecutionEventContainerName,
            (
                logger,
                container,
                serviceProvider) => new CommandExecutionCosmosDbRepository(logger, container, serviceProvider));

        services.AddCommandsAuditPipelineBehavior<CommandAuditingCosmosDbStorage>();
    }
}