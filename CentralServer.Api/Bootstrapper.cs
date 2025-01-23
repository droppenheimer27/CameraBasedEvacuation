using System.Text.Json.Serialization;
using CameraBasedEvacuation.Api.Shared.Middlewares;
using CentralServer.Application.Extensions;
using CentralServer.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace CentralServer.Actor;

public static class Bootstrapper
{
    public static WebApplication Setup(WebApplicationBuilder builder)
    {
        builder.Services
            .AddEndpointsApiExplorer()
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        SetupDependencies(builder.Services);
        SetupSwagger(builder.Services);

        return SetupApplication(builder.Build());
    }
    
    private static void SetupDependencies(IServiceCollection services)
    {
        services
            .AddInfrastructure()
            .AddApplication();
    }
    
    private static void SetupSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.DescribeAllParametersInCamelCase();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Central Server API",
                Version = "v1",
                Description = "This API facilitates real-time tracking of people on-site using camera data. " +
                              "It aggregates the number of people entering and exiting monitored areas, " +
                              "provides accurate counts of people on-site for evacuation planning. " +
                              "The system processes asynchronous and out-of-order camera updates and ensures reliable real-time analytics.",
                Contact = new OpenApiContact
                {
                    Name = "Uladzislau Pavlowski",
                    Email = "uladzislau.pavlowski@proton.me",
                }
            });
        });
    }
    
    private static WebApplication SetupApplication(WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.DefaultModelExpandDepth(-1);
                config.DefaultModelRendering(ModelRendering.Example);
                config.DisplayRequestDuration();
                config.DocExpansion(DocExpansion.List);
                config.EnableDeepLinking();
                config.ShowExtensions();
                config.ShowCommonExtensions();
                config.EnableValidator();
            });
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        return app;
    }
}