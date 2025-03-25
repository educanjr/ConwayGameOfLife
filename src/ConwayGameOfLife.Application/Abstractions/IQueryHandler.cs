using ConwayGameOfLife.Application.Common;
using MediatR;

namespace ConwayGameOfLife.Application.Abstractions;

/// <summary>
/// Handles a query and returns a result payload.
/// </summary>
/// <typeparam name="TQuery">The query type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, ResultObject<TResponse>> where TQuery : IQuery<TResponse>
{
}

