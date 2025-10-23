namespace Api.StartupExtensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddSwaggerGen();

        return services;
    }
}
