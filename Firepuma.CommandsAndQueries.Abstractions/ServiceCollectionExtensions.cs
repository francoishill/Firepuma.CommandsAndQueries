using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Firepuma.CommandsAndQueries.Abstractions;

public static class ServiceCollectionExtensions
{
    public static void AddCommandsAuditPipelineBehavior<TAuditStorageImplementation>(
        this IServiceCollection services,
        Options options)
        where TAuditStorageImplementation : class, ICommandAuditingStorage
    {
        options ??= new Options();

        services.AddTransient<ICommandAuditingStorage, TAuditStorageImplementation>();

        if (options.AddLoggingScopeBehavior)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingScopeBehaviour<,>));
        }

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditCommandsBehaviour<,>));
    }

    public class Options
    {
        /// <summary>
        /// Add scope (with logger.BeginScope) around command handlers, to indicate the request ty
        /// </summary>
        public bool AddLoggingScopeBehavior { get; set; } = true;
    }
}