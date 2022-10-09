using Firepuma.CommandsAndQueries.Abstractions.Config;
using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Firepuma.CommandsAndQueries.Abstractions;

public static class ServiceCollectionExtensions
{
    public static void AddCommandHandling(
        this IServiceCollection services,
        CommandHandlingOptions commandHandlingOptions)
    {
        if (commandHandlingOptions == null) throw new ArgumentNullException(nameof(commandHandlingOptions));

        if (commandHandlingOptions.AddLoggingScopePipelineBehavior)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingScopePipelineBehaviour<,>));
        }

        if (commandHandlingOptions.AddAuditing)
        {
            if (services.All(svc => svc.ServiceType != typeof(ICommandAuditingStorage)))
            {
                throw new InvalidOperationException(
                    $"An implementation of {nameof(ICommandAuditingStorage)} must be registered before calling {nameof(AddCommandHandling)} with" +
                    $" commandHandlingOptions.{nameof(commandHandlingOptions.AddAuditing)} is true");
            }

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditCommandsBehaviour<,>));
        }
    }
}