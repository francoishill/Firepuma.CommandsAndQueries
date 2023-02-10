using System.Reflection;
using Firepuma.CommandsAndQueries.Abstractions;
using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;
using Firepuma.CommandsAndQueries.MongoDb;
using FluentValidation;
using MediatR;
using Sample.CommandsAndQueriesApi.MongoDb.Configuration;
using Sample.CommandsAndQueriesApi.MongoDb.Plumbing.DomainRequestHandling.PipelineBehaviors;

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

        services.AddMongoDbCommandExecutionRecording(
            mongoDbOptions.CommandExecutionsCollectionName);

        services.AddMongoDbCommandAuthorization(
            mongoDbOptions.AuthorizationFailuresCollectionName);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(WrapCommandExceptionPipeline<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingScopePipeline<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLogPipeline<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandExecutionRecordingPipeline<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PrerequisitesPipelineBehavior<,>));
        services.AddValidatorsFromAssemblies(assembliesWithCommandHandlers, ServiceLifetime.Transient);
        services.AddAuthorizersFromAssemblies(assembliesWithCommandHandlers);
    }
}