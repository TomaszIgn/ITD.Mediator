using ITD.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ITD.Mediator.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddITDMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator>();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        IEnumerable<Type> handlerTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType &&
                (
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                    i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                    i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                )
            ));

        foreach (Type type in handlerTypes)
        {
            foreach (Type @interface in type.GetInterfaces())
            {
                services.AddScoped(@interface, type);
            }
        }

        return services;
    }
}