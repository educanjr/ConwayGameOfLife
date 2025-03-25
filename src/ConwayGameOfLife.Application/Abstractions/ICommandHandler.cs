using ConwayGameOfLife.Application.Common;
using MediatR;

namespace ConwayGameOfLife.Application.Abstractions;

/// <summary>
/// Handles a command that does not return a result payload.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, ResultObject>
    where TCommand : ICommand
{
}

/// <summary>
/// Handles a command that returns a result payload.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, ResultObject<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
