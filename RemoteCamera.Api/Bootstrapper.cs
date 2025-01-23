using System.Text.Json.Serialization;
using CameraBasedEvacuation.Api.Shared.Middlewares;
using Microsoft.OpenApi.Models;
using RemoteCamera.Application.Extensions;
using RemoteCamera.Application.Settings;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace RemoteCamera.Api;

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

        builder.Services
            .Configure<CentralServerActorSystemSettings>(builder.Configuration
            .GetSection(CentralServerActorSystemSettings.Section));
        
        builder.Services.AddSingleton<RemoteCameraSettings>(_ =>
        {
            var cameraId = Environment.GetEnvironmentVariable("CAMERA_ID");
            if (cameraId == null)
            {
                throw new ArgumentNullException(nameof(cameraId));
            }

            return new RemoteCameraSettings { CameraId = cameraId };
        });

        SetupDependencies(builder.Services);
        SetupSwagger(builder.Services);

        return SetupApplication(builder.Build());
    }
    
    private static void SetupDependencies(IServiceCollection services)
    {
        services.AddApplication();
    }
    
    private static void SetupSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.DescribeAllParametersInCamelCase();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Camera Management API",
                Version = "v1",
                Description 
                    = "This API allows you to manage the states of cameras and process updates related to the monitoring of evacuation areas. " +
                      "It accepts camera state updates, including the number of people entering and exiting monitored areas. " +
                      "These updates are communicated to the camera actor, which processes the information and ensures the proper tracking of people on-site.",
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