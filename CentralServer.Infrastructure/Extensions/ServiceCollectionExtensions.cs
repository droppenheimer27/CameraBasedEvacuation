using CentralServer.Domain.CameraUpdates;
using CentralServer.Infrastructure.Persistence;
using CentralServer.Infrastructure.Persistence.CameraUpdates;
using Microsoft.Extensions.DependencyInjection;

namespace CentralServer.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPersistence<CameraUpdate>, InMemoryPersistence<CameraUpdate>>();
        services.AddScoped<ICameraUpdatesRepository, CameraUpdatesRepository>();
        
        return services;
    } 
}