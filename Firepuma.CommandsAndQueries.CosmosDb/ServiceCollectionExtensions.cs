using Firepuma.CommandsAndQueries.Abstractions;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using Firepuma.CommandsAndQueries.CosmosDb.Config;
using Firepuma.CommandsAndQueries.CosmosDb.Entities;
using Firepuma.CommandsAndQueries.CosmosDb.Repositories;
using Firepuma.CommandsAndQueries.CosmosDb.Services;
using Firepuma.DatabaseRepositories.CosmosDb;
using Microsoft.Extensions.DependencyInjection;

namespace Firepuma.CommandsAndQueries.CosmosDb;

public static class ServiceCollectionExtensions
{
    public static void AddCommandHandlingWithCosmosDbStorage(
        this IServiceCollection services,
        CosmosDbCommandHandlingOptions commandHandlingOptions)
    {
        if (commandHandlingOptions == null) throw new ArgumentNullException(nameof(commandHandlingOptions));

        if (commandHandlingOptions.AddRecordingOfExecution)
        {
            services.AddCosmosDbCommandExecutionRecording(
                commandHandlingOptions.CommandExecutionEventContainerName,
                commandHandlingOptions.CommandExecutionPartitionKeyGenerator);
        }

        if (commandHandlingOptions.AddAuthorizationBehaviorPipeline)
        {
            services.AddCosmosDbCommandAuthorization(
                commandHandlingOptions.AuthorizationFailureEventContainerName,
                commandHandlingOptions.AuthorizationFailurePartitionKeyGenerator);
        }

        services.AddCommandHandling(commandHandlingOptions);
    }

    private static void AddCosmosDbCommandExecutionRecording(
        this IServiceCollection services,
        string containerName,
        Type partitionKeyGeneratorImplementationType)
    {
        if (containerName == null) throw new ArgumentNullException(nameof(containerName));
        if (partitionKeyGeneratorImplementationType == null) throw new ArgumentNullException(nameof(partitionKeyGeneratorImplementationType));

        if (!partitionKeyGeneratorImplementationType.IsAssignableTo(typeof(ICommandExecutionPartitionKeyGenerator)))
        {
            throw new InvalidOperationException(
                $"{nameof(partitionKeyGeneratorImplementationType)} must implement interface {nameof(ICommandExecutionPartitionKeyGenerator)}");
        }

        services.AddSingleton(typeof(ICommandExecutionPartitionKeyGenerator), partitionKeyGeneratorImplementationType);

        services.AddCosmosDbRepository<
            CommandExecutionCosmosDbEvent,
            ICommandExecutionRepository,
            CommandExecutionCosmosDbRepository>(
            containerName,
            (logger, container, serviceProvider) => new CommandExecutionCosmosDbRepository(logger, container, serviceProvider));

        services.AddTransient<ICommandExecutionStorage, CommandExecutionCosmosDbStorage>();
    }

    private static void AddCosmosDbCommandAuthorization(
        this IServiceCollection services,
        string containerName,
        Type partitionKeyGeneratorImplementationType)
    {
        if (containerName == null) throw new ArgumentNullException(nameof(containerName));
        if (partitionKeyGeneratorImplementationType == null) throw new ArgumentNullException(nameof(partitionKeyGeneratorImplementationType));

        if (!partitionKeyGeneratorImplementationType.IsAssignableTo(typeof(IAuthorizationFailurePartitionKeyGenerator)))
        {
            throw new InvalidOperationException(
                $"{nameof(partitionKeyGeneratorImplementationType)} must implement interface {nameof(IAuthorizationFailurePartitionKeyGenerator)}");
        }

        services.AddSingleton(typeof(IAuthorizationFailurePartitionKeyGenerator), partitionKeyGeneratorImplementationType);

        services.AddCosmosDbRepository<
            AuthorizationFailureCosmosDbEvent,
            IAuthorizationFailureEventRepository,
            AuthorizationFailureEventCosmosDbRepository>(
            containerName,
            (logger, container, serviceProvider) => new AuthorizationFailureEventCosmosDbRepository(logger, container, serviceProvider));

        services.AddTransient<ICommandAuthorizationStorage, CommandAuthorizationCosmosDbStorage>();
    }
}