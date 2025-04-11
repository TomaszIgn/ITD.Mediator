using ITD.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ITD.Mediator;
public class MediatorBuilder
{
    private readonly IServiceCollection _services;

    private readonly List<Assembly> _assemblies = new();
    private readonly List<Type> _openBehaviors = new();

    public MediatorBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public void RegisterServicesFromAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
    }

    public void RegisterServicesFromAppDomain()
    {
        List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .ToList();

        _assemblies.AddRange(assemblies);
    }

    public void AddOpenBehavior(Type openGenericType)
    {
        if (!openGenericType.IsGenericTypeDefinition)
            throw new ArgumentException("Must be an open generic type (e.g. typeof(CommandValidationBehavior<,>))");

        _openBehaviors.Add(openGenericType);
    }

    internal void Build()
    {
        _services.AddScoped<IMediator, Mediator>();

        foreach (Assembly? assembly in _assemblies.Distinct())
        {
            IEnumerable<Type> handlerTypes = assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                    )
                ));

            foreach (Type handlerType in handlerTypes)
            {
                foreach (Type @interface in handlerType.GetInterfaces())
                {
                    if (@interface.IsGenericType)
                    {
                        Type def = @interface.GetGenericTypeDefinition();

                        if (def == typeof(IRequestHandler<>) ||
                            def == typeof(IRequestHandler<,>) ||
                            def == typeof(INotificationHandler<>))
                        {
                            _services.AddScoped(@interface, handlerType);
                        }
                    }
                }
            }
        }

        foreach (Type? openBehavior in _openBehaviors.Distinct())
        {
            _services.AddScoped(typeof(IPipelineBehavior<,>), openBehavior);
        }

        _services.AddScoped<IPublisher, DefaultPublisher>();
    }
}
