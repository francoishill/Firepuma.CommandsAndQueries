using System.Reflection;
using Firepuma.CommandsAndQueries.Abstractions.Authorization;
using Firepuma.CommandsAndQueries.Abstractions.Config;
using Firepuma.CommandsAndQueries.Abstractions.PipelineBehaviors;
using Firepuma.CommandsAndQueries.Abstractions.Services;
using FluentValidation;
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

        if (commandHandlingOptions.AddWrapCommandExceptionsPipelineBehavior)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(WrapCommandExceptionPipeline<,>));
        }

        if (commandHandlingOptions.AddLoggingScopePipelineBehavior)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingScopePipeline<,>));
        }

        if (commandHandlingOptions.AddPerformanceLoggingPipelineBehavior)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceLogPipeline<,>));
        }

        if (commandHandlingOptions.AddValidationBehaviorPipeline)
        {
            if (commandHandlingOptions.ValidationHandlerMarkerAssemblies == null || commandHandlingOptions.ValidationHandlerMarkerAssemblies.Length == 0)
            {
                throw new InvalidOperationException(
                    $"At least one {nameof(commandHandlingOptions.ValidationHandlerMarkerAssemblies)} is required when" +
                    $" commandHandlingOptions.{nameof(commandHandlingOptions.AddValidationBehaviorPipeline)} is true");
            }

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));
            services.AddValidatorsFromAssemblies(commandHandlingOptions.ValidationHandlerMarkerAssemblies);
        }

        if (commandHandlingOptions.AddAuthorizationBehaviorPipeline)
        {
            if (services.All(svc => svc.ServiceType != typeof(ICommandAuthorizationStorage)))
            {
                throw new InvalidOperationException(
                    $"An implementation of {nameof(ICommandAuthorizationStorage)} must be registered before calling {nameof(AddCommandHandling)} when" +
                    $" commandHandlingOptions.{nameof(commandHandlingOptions.AddAuthorizationBehaviorPipeline)} is true");
            }

            if (commandHandlingOptions.AuthorizationHandlerMarkerAssemblies == null || commandHandlingOptions.AuthorizationHandlerMarkerAssemblies.Length == 0)
            {
                throw new InvalidOperationException(
                    $"At least one {nameof(commandHandlingOptions.AuthorizationHandlerMarkerAssemblies)} is required when" +
                    $" commandHandlingOptions.{nameof(commandHandlingOptions.AddAuthorizationBehaviorPipeline)} is true");
            }

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationPipeline<,>));
            services.AddAuthorizersFromAssemblies(commandHandlingOptions.ValidationHandlerMarkerAssemblies);
        }


        if (commandHandlingOptions.AddAuditing)
        {
            if (services.All(svc => svc.ServiceType != typeof(ICommandAuditingStorage)))
            {
                throw new InvalidOperationException(
                    $"An implementation of {nameof(ICommandAuditingStorage)} must be registered before calling {nameof(AddCommandHandling)} when" +
                    $" commandHandlingOptions.{nameof(commandHandlingOptions.AddAuditing)} is true");
            }

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuditCommandsBehaviour<,>));
        }
    }

    private static void AddAuthorizersFromAssemblies(this IServiceCollection services, Assembly[] assemblies)
    {
        var authorizerType = typeof(IAuthorizer<>);
        foreach (var assembly in assemblies)
        {
            assembly.GetTypesAssignableTo(authorizerType).ForEach(type =>
            {
                foreach (var implementedInterface in type.ImplementedInterfaces)
                {
                    services.AddScoped(implementedInterface, type);
                }
            });
        }
    }

    private static List<TypeInfo> GetTypesAssignableTo(this Assembly assembly, Type compareType)
    {
        var typeInfoList = assembly.DefinedTypes.Where(x => x.IsClass
                                                            && !x.IsAbstract
                                                            && x != compareType
                                                            && x.GetInterfaces()
                                                                .Any(i => i.IsGenericType
                                                                          && i.GetGenericTypeDefinition() == compareType)).ToList();

        return typeInfoList;
    }
}