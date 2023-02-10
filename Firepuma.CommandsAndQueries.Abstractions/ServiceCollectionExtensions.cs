using System.Reflection;
using Firepuma.CommandsAndQueries.Abstractions.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Firepuma.CommandsAndQueries.Abstractions;

public static class ServiceCollectionExtensions
{
    public static void AddAuthorizersFromAssemblies(this IServiceCollection services, Assembly[] assemblies)
    {
        var authorizerType = typeof(IAuthorizer<>);
        foreach (var assembly in assemblies)
        {
            assembly.GetTypesAssignableTo(authorizerType).ForEach(type =>
            {
                foreach (var implementedInterface in type.ImplementedInterfaces)
                {
                    services.AddTransient(implementedInterface, type);
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