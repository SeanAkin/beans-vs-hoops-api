using System.Net;

namespace HoopsVsBeans.Middleware;

public class IpRestrictionMiddleware
{
    private static readonly HashSet<string> AllowedDomains = new()
    {
        "hoopsvsbeans.com"
    };

    private readonly HashSet<string> AllowedIps;
    private readonly RequestDelegate _next;
    public IpRestrictionMiddleware(RequestDelegate next)
    {
        _next = next;

        var whitelistedIps = Environment.GetEnvironmentVariable("WHITELISTED_IPS")?.Split(',') ?? Array.Empty<string>();
        AllowedIps = new HashSet<string>(whitelistedIps.Select(ip => ip.Trim()));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var remoteIp = !string.IsNullOrEmpty(forwardedFor)
            ? IPAddress.Parse(forwardedFor.Split(',')[0].Trim())
            : context.Connection.RemoteIpAddress;

        if (remoteIp is null)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("Invalid remote IP address.");
            return;
        }

        var ipStr = remoteIp.ToString();

        if (AllowedIps.Contains(ipStr) || AllowedDomains.Contains(ipStr))
        {
            await _next(context);
            return;
        }

        foreach (var host in AllowedDomains.Where(d => !IPAddress.TryParse(d, out _)))
        {
            var resolvedIps = await Dns.GetHostAddressesAsync(host);
            if (resolvedIps.Any(ip => ip.ToString() == ipStr))
            {
                await _next(context);
                return;
            }
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Access Forbidden: Your IP is not allowed.");
    }
}
