using HoopsVsBeans.Models;

namespace HoopsVsBeans.ServiceCollectionExtensions;

public static class SecretExtensions
{
    private const string ApiKeyEnvironmentVariable = "BEANS_VS_HOOPS_API_KEY";
    private const string DiscordWebhookUrl = "DISCORD_WEBHOOK_URL";

    public static IServiceCollection AddSecrets(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable) ?? configuration["BEANS_VS_HOOPS_API_KEY"];
        var discordWebhookUrl = Environment.GetEnvironmentVariable(DiscordWebhookUrl) ?? configuration["DISCORD_WEBHOOK_URL"];

        if (apiKey is null)
        {
            throw new NullReferenceException($"Secret {ApiKeyEnvironmentVariable} missing");
        }
        if (discordWebhookUrl is null)
        {
            throw new NullReferenceException($"Secret {DiscordWebhookUrl} missing");
        }
        
        serviceCollection.Configure<DiscordWebhookOptions>(o =>
        {
            o.Url = discordWebhookUrl;
        });

        serviceCollection.Configure<ApiKeyOptions>(o =>
        {
            o.Key = apiKey;
        });
        
        return serviceCollection;
    }
}