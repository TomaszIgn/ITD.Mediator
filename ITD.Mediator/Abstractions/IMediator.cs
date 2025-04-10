namespace ITD.Mediator.Abstractions;
public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default);
}
