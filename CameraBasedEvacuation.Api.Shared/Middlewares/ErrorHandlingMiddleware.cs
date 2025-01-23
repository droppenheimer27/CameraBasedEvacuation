using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CameraBasedEvacuation.Api.Shared.Middlewares;

/// <summary>
/// Middleware for centralized error handling. Converts unhandled exceptions into ProblemDetails
/// responses in a consistent JSON format.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            await ConvertToProblemDetails(context, exception);
        }
    }

    private async Task ConvertToProblemDetails(HttpContext context, Exception exception)
    {
        var problemDetails = exception switch
        {
            ArgumentException or InvalidOperationException => CreateProblemDetails(
                status: StatusCodes.Status400BadRequest, 
                title: "Invalid Request", 
                detail: exception.Message
            ),
            _ => CreateProblemDetails(
                status: StatusCodes.Status500InternalServerError, 
                title: "Internal Server Error", 
                detail: "An unexpected error occurred while processing your request"
            )
        };
        
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        
        await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails);
    }
    
    private static ProblemDetails CreateProblemDetails(int status, string title, string detail) => new()
    {
        Status = status,
        Title = title,
        Detail = detail
    };
}