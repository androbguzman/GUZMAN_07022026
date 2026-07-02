namespace ABGFileProcessorAPI.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string API_KEY_HEADER = "X-API-KEY";
    private const string VALID_API_KEY = "ABG_Secure_Secret_Key_123"; // In production, store this in user secrets or appsettings.json

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Check if header exists
        if (!context.Request.Headers.TryGetValue(API_KEY_HEADER, out var extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("API Key header missing (Expected: X-API-KEY).");
            return;
        }

        // 2. Validate the key
        if (!VALID_API_KEY.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized. Invalid API Key.");
            return;
        }

        // 3. Key is valid, proceed to the next middleware/controller
        await _next(context);
    }
}