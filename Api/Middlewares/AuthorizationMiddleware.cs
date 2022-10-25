using System.Net;

using Accessories.Exceptions;

namespace Api.Middlewares;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _request;

    public AuthorizationMiddleware(RequestDelegate request)
    {
        _request = request;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch (InvalidTokenException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        }
    }
}
