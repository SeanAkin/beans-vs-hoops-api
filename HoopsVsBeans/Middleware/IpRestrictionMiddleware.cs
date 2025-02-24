using System.Net;

namespace HoopsVsBeans.Middleware;

public class IpRestrictionMiddleware(RequestDelegate next, ILogger<IpRestrictionMiddleware> logger)
{
    private static readonly HashSet<string> AllowedDomains =
    [
        "hoopsvsbeans.com"
    ];

    public async Task InvokeAsync(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var remoteIp = !string.IsNullOrEmpty(forwardedFor)
            ? IPAddress.Parse(forwardedFor.Split(',')[0].Trim())
            : context.Connection.RemoteIpAddress;

        logger.LogInformation($"Real IP: {remoteIp} requested: {context.Request.Method} {context.Request.Path}");

        if (remoteIp is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Invalid remote IP address.");
            return;
        }

        var ipStr = remoteIp.ToString();

        if (AllowedDomains.Contains(ipStr))
        {
            await next(context);
            return;
        }

        foreach (var host in AllowedDomains.Where(d => !IPAddress.TryParse(d, out _)))
        {
            var resolvedIps = await Dns.GetHostAddressesAsync(host);
            if (resolvedIps.Any(ip => ip.ToString() == ipStr))
            {
                await next(context);
                return;
            }
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Access Forbidden: Your IP is not allowed.");
    }
}
