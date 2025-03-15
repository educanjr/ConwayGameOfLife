using System.ComponentModel.DataAnnotations;

namespace ConwayGameOfLife.Application.Common;

public class ResultError
{
    [Required]
    public ErrorCode Code { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? Target { get; set; }

    public IEnumerable<ResultError>? Details { get; set; }
}

public enum ErrorCode
{
    InternalError,
    NotFound
}
