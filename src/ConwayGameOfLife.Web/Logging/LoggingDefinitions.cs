using Microsoft.Extensions.Logging;

namespace ConwayGameOfLife.Web.Logging;

internal static partial class LoggingDefinitions
{
    [LoggerMessage(
        EventId = 1003,
        Level = LogLevel.Warning,
        Message = "Expected failure occurred: {Message}")]
    public static partial void LogExpectedFailure(this ILogger logger, string message);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Error,
        Message = "Unexpected failure occurred: {Message}")]
    public static partial void LogError(this ILogger logger, string message, Exception ex);
}
