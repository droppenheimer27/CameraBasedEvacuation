using Akka.Hosting;
using Akka.Pattern;
using Akka.Remote.Hosting;
using CentralServer.Application.Actors;
using CentralServer.Application.CameraUpdates;
using Microsoft.Extensions.DependencyInjection;

namespace CentralServer.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config => 
        {
            config.RegisterServicesFromAssembly(typeof(AddCameraUpdateCommand).Assembly);
        });
        
        services.AddActors();
        
        return services;
    }
    
    private static IServiceCollection AddActors(this IServiceCollection services)
    {
        services.AddAkka(Constants.ActorSystemName, builder =>
        {
            // It would be better to put environment variables below to other configuration
            // But for demonstration needs it is fine
            
            var port = Environment.GetEnvironmentVariable("ACTOR_PORT");
            if (port is null)
            {
                throw new ArgumentNullException(nameof(port));
            }
            
            var hostname = Environment.GetEnvironmentVariable("ACTOR_HOSTNAME");
            if (hostname is null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            builder
                .WithRemoting(hostname, port: int.Parse(port))
                .WithActors((system, registry, resolver) =>
                {
                    var supervisorProps = BackoffSupervisor.Props(
                        Backoff.OnFailure(
                            resolver.Props<CameraMonitor>(),
                            childName: nameof(CameraMonitor),
                            maxNrOfRetries: 3,
                            minBackoff: TimeSpan.FromSeconds(3),
                            maxBackoff: TimeSpan.FromSeconds(30),
                            randomFactor: 0.2
                        )
                    );
                    
                    var supervisor = system.ActorOf(supervisorProps, nameof(CameraMonitor));
                    registry.Register<BackoffSupervisor>(supervisor);
                });
        });
        
        return services;
    }
}