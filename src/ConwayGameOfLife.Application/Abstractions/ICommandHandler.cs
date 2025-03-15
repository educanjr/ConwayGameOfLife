using ConwayGameOfLife.Application.Common;
using MediatR;

namespace ConwayGameOfLife.Application.Abstractions;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, ResultObject>
    where TCommand : ICommand
{
}

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, ResultObject<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
