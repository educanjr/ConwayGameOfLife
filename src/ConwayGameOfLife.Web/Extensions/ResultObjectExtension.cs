using ConwayGameOfLife.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace ConwayGameOfLife.Web.Extensions;

internal static class ResultObjectExtension
{
    internal static async Task<ResultObject> Bind<TIn>(
        this ResultObject<TIn> result,
        Func<TIn, Task<ResultObject>> predicate)
    {
        return result.IsSuccess ?
            await predicate(result.Value) :
            ResultObject.Failure(result.ErrorResult!);
    }

    internal static async Task<ResultObject<TOut>> Bind<TIn, TOut>(
        this ResultObject<TIn> result,
        Func<TIn, Task<ResultObject<TOut>>> predicate)
    {
        return result.IsSuccess ? 
            await predicate(result.Value) :
            ResultObject.Failure<TOut>(result.ErrorResult!);
    }

    internal static async Task<IActionResult> Match(
        this Task<ResultObject> resultTask,
        Func<IActionResult> onSuccess,
        Func<ResultError, IActionResult> onFailure)
    {
        ResultObject result = await resultTask;
        return result.IsSuccess ? onSuccess() : onFailure(result.ErrorResult!);
    }

    internal static async Task<IActionResult> Match<TIn>(
        this Task<ResultObject<TIn>> resultTask,
        Func<TIn, IActionResult> onSuccess,
        Func<ResultError, IActionResult> onFailure)
    {
        ResultObject<TIn> result = await resultTask;
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.ErrorResult!);
    }
}
