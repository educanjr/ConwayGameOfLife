using ConwayGameOfLife.Application.Common;
using FluentAssertions;

namespace ConwayGameOfLife.UnitTests.Utils;

internal static class ResultObjectValidations
{
    public static void AssertFailure(this ResultObject resultObject, ErrorCode expectedCode)
    {
        resultObject.IsSuccess.Should().BeFalse();
        resultObject.ErrorResult.Should().NotBeNull();
        resultObject.ErrorResult.Code.Should().Be(expectedCode);
    }

    public static void AssertFailure(this ResultObject resultObject, ErrorCode expectedCode, string expectedMessage)
    {
        resultObject.IsSuccess.Should().BeFalse();
        resultObject.ErrorResult.Should().NotBeNull();
        resultObject.ErrorResult.Code.Should().Be(expectedCode);
        resultObject.ErrorResult.Message.Should().Be(expectedMessage);
    }

    public static void AssertFailure<TResult>(this ResultObject<TResult> resultObject, ErrorCode expectedCode)
    {
        resultObject.IsSuccess.Should().BeFalse();
        resultObject.ErrorResult.Should().NotBeNull();
        resultObject.ErrorResult.Code.Should().Be(expectedCode);
    }

    public static void AssertFailure<TResult>(this ResultObject<TResult> resultObject, ErrorCode expectedCode, string expectedMessage)
    {
        resultObject.IsSuccess.Should().BeFalse();
        resultObject.ErrorResult.Should().NotBeNull();
        resultObject.ErrorResult.Code.Should().Be(expectedCode);
        resultObject.ErrorResult.Message.Should().Be(expectedMessage);
    }

    public static void AssertSuccess(this ResultObject resultObject)
    {
        resultObject.IsSuccess.Should().BeTrue();
        resultObject.ErrorResult.Should().BeNull();
    }

    public static void AssertSuccess<TResult>(this ResultObject<TResult> resultObject)
    {
        resultObject.IsSuccess.Should().BeTrue();
        resultObject.Value.Should().NotBeNull();
        resultObject.ErrorResult.Should().BeNull();
    }
}
