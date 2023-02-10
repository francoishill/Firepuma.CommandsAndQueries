using Firepuma.CommandsAndQueries.Abstractions.Services;
using Firepuma.CommandsAndQueries.MongoDb.Entities;
using Firepuma.CommandsAndQueries.MongoDb.Repositories;
using Firepuma.CommandsAndQueries.MongoDb.Services;
using Firepuma.DatabaseRepositories.MongoDb;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable ArgumentsStyleNamedExpression

namespace Firepuma.CommandsAndQueries.MongoDb;

public static class ServiceCollectionExtensions
{
    public static void AddMongoDbCommandExecutionRecording(
        this IServiceCollection services,
        string collectionName)
    {
        if (collectionName == null) throw new ArgumentNullException(nameof(collectionName));

        services.AddMongoDbRepository<
            CommandExecutionMongoDbEvent,
            ICommandExecutionRepository,
            CommandExecutionMongoDbRepository>(
            collectionName,
            (logger, collection, _) => new CommandExecutionMongoDbRepository(logger, collection),
            indexesFactory: CommandExecutionMongoDbEvent.GetSchemaIndexes);

        services.AddTransient<ICommandExecutionStorage, CommandExecutionMongoDbStorage>();
    }

    public static void AddMongoDbCommandAuthorization(
        this IServiceCollection services,
        string collectionName)
    {
        if (collectionName == null) throw new ArgumentNullException(nameof(collectionName));

        services.AddMongoDbRepository<
            AuthorizationFailureMongoDbEvent,
            IAuthorizationFailureEventRepository,
            AuthorizationFailureEventMongoDbRepository>(
            collectionName,
            (logger, collection, _) => new AuthorizationFailureEventMongoDbRepository(logger, collection),
            indexesFactory: AuthorizationFailureMongoDbEvent.GetSchemaIndexes);

        services.AddTransient<ICommandAuthorizationStorage, CommandAuthorizationMongoDbStorage>();
    }
}