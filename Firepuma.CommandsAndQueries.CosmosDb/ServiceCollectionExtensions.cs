using Firepuma.CommandsAndQueries.Abstractions;
using Firepuma.CommandsAndQueries.Abstractions.Entities;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using Firepuma.CommandsAndQueries.CosmosDb.Config;
using Firepuma.CommandsAndQueries.CosmosDb.Repositories;
using Firepuma.CommandsAndQueries.CosmosDb.Services;
using Firepuma.DatabaseRepositories.CosmosDb;
using Microsoft.Extensions.DependencyInjection;

namespace Firepuma.CommandsAndQueries.CosmosDb;

public static class ServiceCollectionExtensions
{
    public static void AddCommandHandlingWithCosmosDbAuditing(
        this IServiceCollection services,
        CosmosDbCommandHandlingOptions commandHandlingOptions)
    {
        if (commandHandlingOptions == null) throw new ArgumentNullException(nameof(commandHandlingOptions));

        if (!commandHandlingOptions.AddAuditing)
        {
            throw new InvalidOperationException(
                $"{nameof(AddCommandHandlingWithCosmosDbAuditing)} cannot be called with commandHandlingOptions.{nameof(commandHandlingOptions.AddAuditing)} as false, please" +
                $" consider calling the {nameof(Firepuma.CommandsAndQueries.Abstractions.ServiceCollectionExtensions.AddCommandHandling)} method directly");
        }

        services.AddCosmosDbAuditing(
            commandHandlingOptions.CommandExecutionEventContainerName,
            commandHandlingOptions.CommandAuditPartitionKeyGenerator);

        services.AddCommandHandling(commandHandlingOptions);
    }

    private static void AddCosmosDbAuditing(
        this IServiceCollection services,
        string commandExecutionEventContainerName,
        Type commandAuditPartitionKeyGenerator)
    {
        if (commandExecutionEventContainerName == null) throw new ArgumentNullException(nameof(commandExecutionEventContainerName));
        if (commandAuditPartitionKeyGenerator == null) throw new ArgumentNullException(nameof(commandAuditPartitionKeyGenerator));

        if (!commandAuditPartitionKeyGenerator.IsAssignableTo(typeof(ICommandAuditPartitionKeyGenerator)))
        {
            throw new InvalidOperationException(
                $"{nameof(commandAuditPartitionKeyGenerator)} must implement interface {nameof(ICommandAuditPartitionKeyGenerator)}");
        }

        services.AddSingleton(typeof(ICommandAuditPartitionKeyGenerator), commandAuditPartitionKeyGenerator);

        services.AddCosmosDbRepository<
            CommandExecutionEvent,
            ICommandExecutionRepository,
            CommandExecutionCosmosDbRepository>(
            commandExecutionEventContainerName,
            (
                logger,
                container,
                serviceProvider) => new CommandExecutionCosmosDbRepository(logger, container, serviceProvider));

        services.AddTransient<ICommandAuditingStorage, CommandAuditingCosmosDbStorage>();
    }
}