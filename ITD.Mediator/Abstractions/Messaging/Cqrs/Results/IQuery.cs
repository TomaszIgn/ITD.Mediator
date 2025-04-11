using ITD.Mediator.Abstractions.Messaging.Cqrs;
using ITD.Results;

namespace ITD.Mediator.Abstractions.Messaging.Cqrs.Results;
public interface IQuery<TResponse> : IRequest<Result<TResponse>>, IQueryBase
{
}
