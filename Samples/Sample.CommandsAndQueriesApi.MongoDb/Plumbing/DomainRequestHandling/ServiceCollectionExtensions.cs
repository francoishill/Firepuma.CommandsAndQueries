using System.Reflection;
using Firepuma.CommandsAndQueries.Abstractions;
using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;
using Firepuma.DatabaseRepositories.MongoDb;
using FluentValidation;
using MediatR;
using Sample.CommandsAndQueriesApi.MongoDb.Configuration;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Entities;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.PipelineBehaviors;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.Repositories;

namespace Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling;

public static class ServiceCollectionExtensions
{
    public static void AddCommandHandlingMediatRStorageAndPipelines(
        this IServiceCollection services,
        MongoDbOptions mongoDbOptions,
        Assembly[] assembliesWithCommandHandlers)
    {
        if (mongoDbOptions == null) throw new ArgumentNullException(nameof(mongoDbOptions));
        if (assembliesWithCommandHandlers.Length == 0) throw new ArgumentException("At least one assembly is required", nameof(assembliesWithCommandHandlers));

        var duplicateAssemblies = assembliesWithCommandHandlers.GroupBy(a => a).Where(group => group.Count() > 1).ToArray();
        if (duplicateAssemblies.Length > 0)
        {
            throw new ArgumentException(
                $"Duplicate assemblies found: {string.Join("; ", duplicateAssemblies.Select(a => a.Key.FullName))}",
                nameof(assembliesWithCommandHandlers));
        }

        services.AddMongoDbRepository<
            CommandExecutionMongoDbEvent,
            ICommandExecutionRepository,
            CommandExecutionMongoDbRepository>(
            mongoDbOptions.CommandExecutionsCollectionName,
            (logger, collection, _) => new CommandExecutionMongoDbRepository(logger, collection),
            indexesFactory: CommandExecutionMongoDbEvent.GetSchemaIndexes);

        services.AddMongoDbRepository<
            AuthorizationFailureMongoDbEvent,
            IAuthorizationFailureEventRepository,
            AuthorizationFailureEventMongoDbRepository>(
            mongoDbOptions.AuthorizationFailuresCollectionName,
            (logger, collection, _) => new AuthorizationFailureEventMongoDbRepository(logger, collection),
            indexesFactory: AuthorizationFailureMongoDbEvent.GetSchemaIndexes);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(WrapCommandExceptionPipeline<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingScopePipeline<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLogPipeline<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PrerequisitesPipelineBehavior<,>));
        services.AddValidatorsFromAssemblies(assembliesWithCommandHandlers, ServiceLifetime.Transient);
        services.AddAuthorizersFromAssemblies(assembliesWithCommandHandlers);

        // Add this after prerequisites because we don't want to record command execution events for validation/authorization failures
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandExecutionRecordingPipeline<,>));
    }
}