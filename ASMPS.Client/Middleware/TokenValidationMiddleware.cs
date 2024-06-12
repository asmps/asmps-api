namespace ASMPS.Client.Middleware;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies["JWToken"];
        if (string.IsNullOrEmpty(token) && context.User.Identity!.IsAuthenticated)
        {
            context.Response.Redirect("/Account/Login");
            return;
        }

        await _next(context);
    }
}