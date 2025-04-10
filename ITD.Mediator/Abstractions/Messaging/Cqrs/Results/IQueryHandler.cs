using ITD.Results;

namespace ITD.Mediator.Abstractions.Messaging.Cqrs.Results;
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>;

