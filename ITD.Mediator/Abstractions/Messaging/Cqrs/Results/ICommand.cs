using ITD.Results;

namespace ITD.Mediator.Abstractions.Messaging.Cqrs.Results;
public interface ICommand : IRequest<Result>, ICommandBase
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, ICommandBase
{
}
