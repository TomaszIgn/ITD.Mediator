namespace ITD.Mediator.Abstractions.Messaging.Cqrs;

public interface IQuery<TResponse> : IRequest<TResponse>, IQueryBase
{
}