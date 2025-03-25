using ConwayGameOfLife.Application.Common;
using MediatR;

namespace ConwayGameOfLife.Application.Abstractions;

/// <summary>
/// Represents a query that returns a <see cref="ResultObject{TResponse}"/>.
/// Used for read-only operations that return data.
/// </summary>
/// <typeparam name="TResponse">The type of data returned by the query.</typeparam>
public interface IQuery<TResponse> : IRequest<ResultObject<TResponse>>
{
}
