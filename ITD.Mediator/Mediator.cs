using ITD.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ITD.Mediator;
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        Type requestType = request.GetType();
        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        object handler = _serviceProvider.GetRequiredService(handlerType);

        List<object> behaviors = _serviceProvider
            .GetServices(typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse)))
            .Cast<object>()
            .Reverse()
            .ToList();

        RequestHandlerDelegate<TResponse> handlerDelegate = () =>
            ((dynamic)handler).Handle((dynamic)request, cancellationToken);

        foreach (object behavior in behaviors)
        {
            RequestHandlerDelegate<TResponse> next = handlerDelegate;
            handlerDelegate = () =>
                ((dynamic)behavior).Handle((dynamic)request, cancellationToken, next);
        }

        return await handlerDelegate();
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        IEnumerable<INotificationHandler<TNotification>> handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();
        IEnumerable<Task> tasks = handlers.Select(h => h.Handle(notification, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
