namespace ITD.Mediator.Abstractions.Messaging.Cqrs;
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;
