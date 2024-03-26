using Microsoft.Extensions.DependencyInjection;

namespace TransportZone.Air.Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));
		return services;
	}
}