using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TransportZone.Air.Infrastructure.Persistence;

namespace TransportZone.Air.Infrastructure.Services.Background;

internal sealed class MigratorService(ILogger<MigratorService> logger, IServiceProvider serviceProvider) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		try
		{
			logger.LogInformation("Начало миграции базы данных.");
			await using var scope = serviceProvider.CreateAsyncScope();
			await using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			await context.Database.MigrateAsync(stoppingToken);
			logger.LogInformation("Миграция базы данных успешно завершена.");
		}
		catch (Exception ex)
		{
			logger.LogError($"Миграция базы данных завершилась ошибкой. {ex.Message}");
			throw;
		}
	}
}