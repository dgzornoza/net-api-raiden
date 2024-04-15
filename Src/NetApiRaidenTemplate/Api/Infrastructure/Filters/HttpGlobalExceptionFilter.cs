using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace $safeprojectname$.Infrastructure.Filters;

/// <summary>
/// Filter to handle all global http exceptions.
/// </summary>
public class HttpGlobalExceptionFilter : IExceptionFilter
{
    private readonly IWebHostEnvironment environment;
    private readonly ILogger<HttpGlobalExceptionFilter> logger;

    public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
    {
        environment = env;
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, $"Error Message --> {context.Exception.Message}");

        // Create the message and code for the specific exceptions
        int statusCode;
        IDictionary<string, string[]> errors;
        switch (context.Exception)
        {
            case ValidationException exception:

#pragma warning disable S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"
                errors = exception.Errors
                    .Select(item => new { Key = item.PropertyName.Split('.').Last(), Value = item.ErrorCode })
                    .GroupBy(item => item.Key)
                    .ToDictionary(grouped => grouped.Key, grouped => grouped.Select(item => item.Value).ToArray());
#pragma warning restore S6608 // Prefer indexing instead of "Enumerable" methods on types implementing "IList"

                statusCode = StatusCodes.Status400BadRequest;
                break;

            case InvalidOperationException _:
                errors = CreateCommonControlledExceptionError(context.Exception);
                statusCode = StatusCodes.Status400BadRequest;
                break;

            case KeyNotFoundException _:
                errors = CreateCommonControlledExceptionError(context.Exception);
                statusCode = StatusCodes.Status404NotFound;
                break;

            case UnauthorizedAccessException _:
                errors = CreateCommonControlledExceptionError(context.Exception);
                statusCode = StatusCodes.Status401Unauthorized;
                break;

            case DbUpdateException _:
                errors = CreateCommonControlledExceptionError(context.Exception.InnerException ?? context.Exception);
                statusCode = StatusCodes.Status400BadRequest;
                break;

            default:
                errors = new Dictionary<string, string[]>()
                {
                    { "UnknownError", new[] { context.Exception.Message } },
                };

                statusCode = StatusCodes.Status500InternalServerError;

                break;
        }

        var problemDetails = new ValidationProblemDetails(errors)
        {
            Title = Properties.Resources.GlobalErrorTitle,
            Status = statusCode,
        };

        // Add debugging information (only available for development environments)
        if (environment.IsDevelopment())
        {
            problemDetails.Instance = context.HttpContext.Request.Path;
            problemDetails.Type = $"https://httpstatuses.com/{statusCode}";
            problemDetails.Detail = context.Exception.StackTrace;
            problemDetails.Extensions.Add("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
        }

        // Set response
        context.Result = new ObjectResult(problemDetails) { StatusCode = statusCode };
        context.HttpContext.Response.StatusCode = statusCode;

        context.ExceptionHandled = true;
    }

    private static Dictionary<string, string[]> CreateCommonControlledExceptionError(Exception exception)
    {
        return new Dictionary<string, string[]>()
        {
            { "ControlledError", new[] { exception.Message } },
        };
    }
}
