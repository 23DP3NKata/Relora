using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Npgsql;
using Relora.Shared.Domain.Exceptions;

namespace Relora.Host.Middleware;

/// <summary>
/// Represents the global exception middleware class.
/// </summary>
public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment environment)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    /// <summary>
    /// Performs the invoke operation.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception,
                "Unhandled exception occurred while processing request {Path}. Full error: {Error}",
                context.Request.Path,
                exception.ToString());

            var problemDetails = BuildProblemDetails(context, exception);

            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    /// <summary>
    /// Performs the build problem details operation.
    /// </summary>
    /// <param name="context">Context.</param>
    /// <param name="exception">Exception.</param>
    /// <returns>The operation result.</returns>
    private ProblemDetails BuildProblemDetails(HttpContext context, Exception exception)
    {
        if (exception is ValidationException validationException)
        {
            return BuildValidationProblemDetails(context, validationException);
        }

        var (statusCode, title, message) = exception switch
        {
            Error applicationError => (
                applicationError.StatusCode,
                applicationError.StatusCode == StatusCodes.Status409Conflict ? "Conflict" : "Bad Request",
                applicationError.Message),
            UnauthorizedAccessException => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                exception.Message),
            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Not Found",
                exception.Message),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                exception.Message),
            InvalidOperationException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                exception.Message),
            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "The data changed before your request could be saved. Refresh and try again."),
            DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } } => (
                StatusCodes.Status409Conflict,
                "Conflict",
                "A record with these values already exists. Refresh and try again."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = $"https://httpstatuses.com/{statusCode}",
            Detail = _environment.IsDevelopment() && statusCode == StatusCodes.Status500InternalServerError
                ? exception.ToString()
                : message,
            Instance = context.Request.Path
        };

        problemDetails.Extensions["message"] = message;
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        return problemDetails;
    }

    private static ValidationProblemDetails BuildValidationProblemDetails(
        HttpContext context,
        ValidationException exception)
    {
        var errors = exception.Errors
            .Where(error => error is not null)
            .GroupBy(error =>
                string.IsNullOrWhiteSpace(error.PropertyName)
                    ? "general"
                    : error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToArray());

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Type = $"https://httpstatuses.com/{StatusCodes.Status400BadRequest}",
            Detail = "One or more validation errors occurred.",
            Instance = context.Request.Path
        };

        problemDetails.Extensions["message"] = "One or more validation errors occurred.";
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        return problemDetails;
    }
}
