using System.Reflection;
using Firepuma.CommandsAndQueries.Abstractions;
using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;
using Firepuma.CommandsAndQueries.CosmosDb;
using FluentValidation;
using MediatR;
using Sample.CommandsAndQueriesApi.CosmosDb.Configuration;
using Sample.CommandsAndQueriesApi.CosmosDb.Plumbing.DomainRequestHandling.PipelineBehaviors;
using Sample.CommandsAndQueriesApi.CosmosDb.Services;

namespace Sample.CommandsAndQueriesApi.CosmosDb.Plumbing.DomainRequestHandling;

public static class ServiceCollectionExtensions
{
    public static void AddCommandHandlingMediatRStorageAndPipelines(
        this IServiceCollection services,
        Assembly[] assembliesWithCommandHandlers)
    {
        if (assembliesWithCommandHandlers.Length == 0) throw new ArgumentException("At least one assembly is required", nameof(assembliesWithCommandHandlers));

        var duplicateAssemblies = assembliesWithCommandHandlers.GroupBy(a => a).Where(group => group.Count() > 1).ToArray();
        if (duplicateAssemblies.Length > 0)
        {
            throw new ArgumentException(
                $"Duplicate assemblies found: {string.Join("; ", duplicateAssemblies.Select(a => a.Key.FullName))}",
                nameof(assembliesWithCommandHandlers));
        }

        services.AddCosmosDbCommandExecutionRecording(
            CosmosContainers.CommandExecutions.ContainerProperties.Id,
            typeof(CommandExecutionPartitionKeyGenerator));

        services.AddCosmosDbCommandAuthorization(
            CosmosContainers.AuthorizationFailures.ContainerProperties.Id,
            typeof(AuthorizationFailurePartitionKeyGenerator));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(WrapCommandExceptionPipeline<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingScopePipeline<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLogPipeline<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CommandExecutionRecordingPipeline<,>));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PrerequisitesPipelineBehavior<,>));
        services.AddValidatorsFromAssemblies(assembliesWithCommandHandlers, ServiceLifetime.Transient);
        services.AddAuthorizersFromAssemblies(assembliesWithCommandHandlers);
    }
}