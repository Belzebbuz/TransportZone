using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using TransportZone.Air.Domain.Airports;
using TransportZone.Air.Domain.Flights;
using TransportZone.Air.Infrastructure.Persistence;

namespace TransportZone.Air.Infrastructure.Services.Background;

internal sealed class FlightsScheduler(
	AppDbContext dbContext, 
	ILogger<FlightsScheduler> logger, 
	IServiceProvider serviceProvider) : IJob
{
	public async Task Execute(IJobExecutionContext context)
	{
		var retryCount = 0;
		while (retryCount < 3)
		{
			if (!await dbContext.Airports.AnyAsync())
			{
				await Task.Delay(5000);
				retryCount++;
				continue;
			}
		
			var nextDay = DateTime.UtcNow.AddDays(2);
			var nextDayDateTime = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day).AddTicks(-1);
			var nextDayUtc = DateTime.SpecifyKind(nextDayDateTime, DateTimeKind.Utc);
			var flightExists = await dbContext.Flights.AnyAsync(
				x => x.ScheduledDeparture >= DateTime.UtcNow && x.ScheduledDeparture <= nextDayUtc);
			if (flightExists)
			{
				logger.LogWarning("Расписание на неделю уже составлено");
				return;
			}
		
			if (!await dbContext.Aircrafts.AnyAsync())
			{
				logger.LogWarning("Не найдено ни одного самолета");
				return;
			}
		
			var aircrafts = await dbContext.Aircrafts
				.AsNoTracking()
				.OrderByDescending(x => x.Range)
				.ToArrayAsync();
			await using var scope = serviceProvider.CreateAsyncScope();
			await using var secondDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			await using var scopeTwo = serviceProvider.CreateAsyncScope();
			await using var thirdDbContext = scopeTwo.ServiceProvider.GetRequiredService<AppDbContext>();
			var flights = new List<Flight>();
			await foreach (var airport in dbContext.Airports.AsAsyncEnumerable())
			{
				Expression<Func<Airport, bool>> filter = target => EF.Functions
					.DistanceKnn(target.Coordinates, airport.Coordinates) / 1000 <= aircrafts[0].Range
					&& EF.Functions
						.DistanceKnn(target.Coordinates, airport.Coordinates) / 1000 != 0;
				
				var nearestAirports = secondDbContext.Airports.Where(filter)
					.Select(x => new
					{
						x.Id,
						Distance = EF.Functions
							.DistanceKnn(x.Coordinates, airport.Coordinates) / 1000
					})
					.AsNoTracking()
					.OrderBy(x => x.Distance)
					.AsAsyncEnumerable();
				await foreach (var nearestAirport in nearestAirports)
				{
					var minDistanceAircraft = aircrafts
						.Where(x => x.Range - nearestAirport.Distance >= 0)
						.MinBy(x => x.Range - nearestAirport.Distance);
					var flightTime = nearestAirport.Distance * 60 / 850d;
					if (minDistanceAircraft is null)
					{
						logger.LogWarning("Не найден самолет с кратчайшим расстоянием.");
						continue;
					}
					flights.AddRange(CreateFlights(flightTime, airport.Id, nearestAirport.Id, minDistanceAircraft.Id, nextDayUtc));
					logger.LogInformation($"Создано расписание на 2 день. {airport.Id} -> {nearestAirport.Id}. Модель {minDistanceAircraft.Id}");
				}
			}
			await dbContext.Flights.AddRangeAsync(flights);
			await dbContext.SaveChangesAsync();
			logger.LogInformation("Создание расписания завершено");
			return;
		}
	}

	private IEnumerable<Flight> CreateFlights(double flightCount, string airportId, string nearestAirportId,
		string aircraftId, DateTime lastDateTime)
	{
		var nextHour = DateTime.UtcNow.AddMinutes(20);
		var startDate = new DateTime(nextHour.Year, nextHour.Month, nextHour.Day, nextHour.Hour, nextHour.Minute, 0);
		var startDateUtc = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
		for (var date = startDateUtc; date <= lastDateTime; date = date.AddHours(8))
		{
			var flight = Flight.Create(
				date,
				date.AddMinutes(flightCount),
				airportId,
				nearestAirportId,
				aircraftId);
			if (flight.IsError)
			{
				logger.LogError($"Не удалось создать рейс.\n{flight.FirstError.Description}");
				continue;
			}
	
			yield return flight.Value;
		}
	}
}