using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Firepuma.CommandsAndQueries.Abstractions.IntegrationEvents;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Services;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents;

public static class ServiceCollectionExtensions
{
    public static void AddIntegrationEvents(this IServiceCollection services)
    {
        services.AddTransient<ICommandExecutionDecorator, CommandExecutionIntegrationEventDecorator>();
        services.AddTransient<ICommandExecutionIntegrationEventPublisher, CommandExecutionIntegrationEventPublisher>();
        services.AddTransient<ICommandExecutionPostProcessor, CommandExecutionPostProcessor>();

        services.AddHostedService<BackgroundEventPublisherService>();
    }
}