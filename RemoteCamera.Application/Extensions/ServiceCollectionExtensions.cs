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
                    var remoteCamera = system.ActorOf(resolver.Props<Actors.RemoteCamera>());
                    registry.Register<Actors.RemoteCamera>(remoteCamera);
                });
        });
        
        return services;
    }
}