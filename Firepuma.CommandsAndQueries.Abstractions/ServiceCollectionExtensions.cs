using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Firepuma.CommandsAndQueries.Abstractions;

public static class ServiceCollectionExtensions
{
    public static void AddCommandsAuditPipelineBehavior<TAuditStorageImplementation>(
        this IServiceCollection services)
        where TAuditStorageImplementation : class, ICommandAuditingStorage
    {
        services.AddTransient<ICommandAuditingStorage, TAuditStorageImplementation>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditCommandsBehaviour<,>));
    }
}