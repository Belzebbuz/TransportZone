using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetTopologySuite.Geometries;
using TransportZone.Air.Application.Abstractions.HttpClients;
using TransportZone.Air.Domain.Airports;
using TransportZone.Air.Infrastructure.Persistence;

namespace TransportZone.Air.Infrastructure.Services.Background;

public class AirportMigratorService(IServiceProvider provider) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		stoppingToken.ThrowIfCancellationRequested();
		await using var scope = provider.CreateAsyncScope();
		var client = scope.ServiceProvider.GetRequiredService<IApiNinjaClient>();
		await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
		while (!stoppingToken.IsCancellationRequested)
		{
			if (!await context.Database.CanConnectAsync(stoppingToken))
			{
				await Task.Delay(5000, stoppingToken);
				continue;
			}

			if (await context.Airports.AnyAsync(stoppingToken)) 
				break;
			var airports = await client.GetAirportsAsync("RU");
			if(airports.IsError)
				break;
			var entities = airports.Value
				.Where(x => !string.IsNullOrEmpty(x.Iata))
				.Select(x =>
			{
				var point = new Point(new(double.Parse(x.Longitude, CultureInfo.InvariantCulture), double.Parse(x.Latitude, CultureInfo.InvariantCulture)));
				var airport = Airport.Create(x.Iata, x.Name, x.City, point, x.Timezone, x.Country);
				if (airport.IsError)
					return null;
				return airport.Value;
			}).OfType<Airport>();
			await context.Airports.AddRangeAsync(entities, stoppingToken);
			await context.SaveChangesAsync(stoppingToken);
			break;
		}
	}
}