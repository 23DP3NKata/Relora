namespace Relora.Shared.Domain.Results;

/// <summary>
/// Represents the result class.
/// </summary>
public sealed class Result<T>
{
    /// <summary>
    /// Gets or sets the is success used by this type.
    /// </summary>
    public bool IsSuccess { get; }
    /// <summary>
    /// Gets or sets the value used by this type.
    /// </summary>
    public T? Value { get; }
    /// <summary>
    /// Gets or sets the error code used by this type.
    /// </summary>
    public string? ErrorCode { get; }
    /// <summary>
    /// Gets or sets the message used by this type.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Is success.</param>
    /// <param name="value">Value.</param>
    /// <param name="errorCode">Error code.</param>
    /// <param name="message">Message.</param>
    private Result(bool isSuccess, T? value, string? errorCode, string? message)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorCode = errorCode;
        Message = message;
    }

    /// <summary>
    /// Performs the success operation.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <returns>The operation result.</returns>
    public static Result<T> Success(T value)
    {
        return new(true, value, null, null);
    }
    /// <summary>
    /// Performs the fail operation.
    /// </summary>
    /// <param name="errorCode">Error code.</param>
    /// <param name="message">Message.</param>
    /// <returns>The operation result.</returns>
    public static Result<T> Fail(string errorCode, string message)
    {
        return new(false, default, errorCode, message);
    }
}
