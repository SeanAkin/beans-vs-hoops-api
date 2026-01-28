using System.Net;

namespace HoopsVsBeans.Middleware;

public class IpRestrictionMiddleware
{
    private static readonly HashSet<string> AllowedDomains = new()
    {
        "hoopsvsbeans.com"
    };

    private static readonly HashSet<string> AllowedOrigins = new()
    {
        "https://hoopsvsbeans.com",
        "https://www.hoopsvsbeans.com",
        "https://beans-vs-hoops.vercel.app"
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

        // First, try IP-based validation (for whitelisted IPs)
        if (AllowedIps.Contains(ipStr) || AllowedDomains.Contains(ipStr))
        {
            await _next(context);
            return;
        }

        // Try DNS resolution for domains (for static IPs)
        foreach (var host in AllowedDomains.Where(d => !IPAddress.TryParse(d, out _)))
        {
            var resolvedIps = await Dns.GetHostAddressesAsync(host);
            if (resolvedIps.Any(ip => ip.ToString() == ipStr))
            {
                await _next(context);
                return;
            }
        }

        // Fall back to Origin/Referer header validation (for Vercel and other platforms with dynamic IPs)
        // This allows requests from trusted frontend domains even when IP validation fails
        var origin = context.Request.Headers["Origin"].FirstOrDefault();
        var referer = context.Request.Headers["Referer"].FirstOrDefault();

        // Check Origin header first (preferred for CORS requests)
        if (!string.IsNullOrEmpty(origin) && AllowedOrigins.Contains(origin))
        {
            await _next(context);
            return;
        }

        // Fall back to Referer header (for non-CORS requests)
        if (!string.IsNullOrEmpty(referer))
        {
            var refererUri = new Uri(referer);
            var refererOrigin = $"{refererUri.Scheme}://{refererUri.Host}";

            if (AllowedOrigins.Contains(refererOrigin))
            {
                await _next(context);
                return;
            }
        }

        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Access Forbidden: Your IP or origin is not allowed.");
    }
}
