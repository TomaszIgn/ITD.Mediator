namespace ITD.Mediator.Abstractions.Messaging.Cqrs;

public interface ICommand : IRequest, ICommandBase
{
}

public interface ICommand<TResponse> : IRequest<TResponse>, ICommandBase
{
}