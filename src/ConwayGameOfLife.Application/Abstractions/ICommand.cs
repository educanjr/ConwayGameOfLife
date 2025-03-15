using ConwayGameOfLife.Application.Common;
using MediatR;

namespace ConwayGameOfLife.Application.Abstractions;

public interface ICommand : IRequest<ResultObject>
{
}

public interface ICommand<TResponse> : IRequest<ResultObject<TResponse>>
{
}
