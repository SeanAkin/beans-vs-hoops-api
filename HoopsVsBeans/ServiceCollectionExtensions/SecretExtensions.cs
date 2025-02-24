using HoopsVsBeans.Models;

namespace HoopsVsBeans.ServiceCollectionExtensions;

public static class SecretExtensions
{
    private const string ApiKeyEnvironmentVariable = "BEANS_VS_HOOPS_API_KEY";

    public static IServiceCollection AddSecrets(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable) ?? configuration["BEANS_VS_HOOPS_API_KEY"];

        if (apiKey is null)
        {
            throw new NullReferenceException($"Secret {ApiKeyEnvironmentVariable} missing");
        }

        serviceCollection.Configure<ApiKeyOptions>(o =>
        {
            o.Key = apiKey;
        });
        
        return serviceCollection;
    }
}