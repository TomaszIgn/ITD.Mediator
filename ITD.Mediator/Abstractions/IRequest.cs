namespace ITD.Mediator.Abstractions;
public interface IRequest
{
}

public interface IRequest<TResponse> : IRequest
{
}