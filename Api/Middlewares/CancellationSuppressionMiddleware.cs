namespace Api.Middlewares;

internal class CancellationSuppressionMiddleware
{
    private readonly RequestDelegate _next;

    public CancellationSuppressionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // Forward to next middleware
            await _next(httpContext);
        }
        catch (OperationCanceledException)
        {
            if (httpContext.Response.HasStarted) return;

            // Set a status code. Response will likely not be seen by client as we expect all cancellations to come from httpContext.RequestAborted
            // Why 204 No Content? I could not find any other appropriate status code.
            httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
        }
    }
}
