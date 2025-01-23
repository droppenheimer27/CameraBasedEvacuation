using Akka.Hosting;
using Akka.Pattern;
using Akka.Remote.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace RemoteCamera.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddActors();
        return services;
    }

    private static IServiceCollection AddActors(this IServiceCollection services)
    {
        services.AddAkka(Constants.ActorSystemName, builder =>
        {
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
                            resolver.Props<Actors.RemoteCamera>(),
                            childName: nameof(Actors.RemoteCamera),
                            maxNrOfRetries: 3,
                            minBackoff: TimeSpan.FromSeconds(3),
                            maxBackoff: TimeSpan.FromSeconds(30),
                            randomFactor: 0.2
                        )
                    );
                    
                    var supervisor = system.ActorOf(supervisorProps, nameof(Actors.RemoteCamera));
                    registry.Register<BackoffSupervisor>(supervisor);
                });
        });
        
        return services;
    }
}