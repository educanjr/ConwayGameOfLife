using ConwayGameOfLife.Application.Common;
using MediatR;

namespace ConwayGameOfLife.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<ResultObject<TResponse>>
{
}
