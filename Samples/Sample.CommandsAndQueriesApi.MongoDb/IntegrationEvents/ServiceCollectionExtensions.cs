using Firepuma.CommandsAndQueries.Abstractions.Commands;
using Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents.Services;

namespace Sample.CommandsAndQueriesApi.MongoDb.IntegrationEvents;

public static class ServiceCollectionExtensions
{
    public static void AddIntegrationEvents(this IServiceCollection services)
    {
        services.AddTransient<ICommandExecutionDecorator, CommandExecutionIntegrationEventDecorator>();

        services.AddHostedService<BackgroundEventPublisherService>();
    }
}