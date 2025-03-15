using ConwayGameOfLife.Application.Common;
using MediatR;

namespace ConwayGameOfLife.Application.Abstractions;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, ResultObject<TResponse>> where TQuery : IQuery<TResponse>
{
}

