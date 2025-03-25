using ConwayGameOfLife.Application.Common;
using MediatR;

namespace ConwayGameOfLife.Application.Abstractions;

/// <summary>
/// Represents a command that returns a <see cref="ResultObject"/>.
/// Used for operations that perform actions but do not return data.
/// </summary>
public interface ICommand : IRequest<ResultObject>
{
}

/// <summary>
/// Represents a command that returns a <see cref="ResultObject{TResponse}"/>.
/// Used for actions that produce a result.
/// </summary>
/// <typeparam name="TResponse">The type of data returned by the command.</typeparam>
public interface ICommand<TResponse> : IRequest<ResultObject<TResponse>>
{
}
