using ITD.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ITD.Mediator;
internal class DefaultPublisher : IPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        INotificationHandler<TNotification>? handler = _serviceProvider.GetService<INotificationHandler<TNotification>>();

        if (handler == null)
            throw new InvalidOperationException($"No handler registered for {typeof(TNotification).Name}");

        await handler.Handle(notification, cancellationToken);
    }
}
