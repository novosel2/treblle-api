using Application.Interfaces.IServices;
using Application.Interfaces.IClients;
using Infrastructure.Clients;
using Infrastructure.HostedServices;
using Application.Services;
using Microsoft.OpenApi.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories;
using Application.Interfaces.IRepositories;
using System.Text.Json.Serialization;
using System.Text.Json;
using Api.Filters.ExceptionFilters;

namespace Api.StartupExtensions;

public static class ConfigureServicesExtension
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(typeof(ExceptionFilter));
        })
        .AddJsonOptions(options =>
        {
            // use names instead of 0/1/2 in JSON
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)); // or null for PascalCase
        });
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "To-Do API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });

        services.AddHttpClient();
        services.AddHttpContextAccessor();

        services.AddDbContext<AppDbContext>(options
                => options.UseNpgsql(config.GetConnectionString("AppConnection")));

        services.AddScoped<IApiClient, ApiClient>();
        services.AddScoped<IProxyService, ProxyService>();
        services.AddScoped<ILogsService, LogsService>();
        services.AddScoped<ILogsRepository, LogsRepository>();

        services.AddHostedService<MigrationHostedService>();

        return services;
    }
}
