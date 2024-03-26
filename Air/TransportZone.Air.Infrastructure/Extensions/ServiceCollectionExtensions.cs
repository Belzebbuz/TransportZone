using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TransportZone.Air.Application.Abstractions;
using TransportZone.Air.Application.Abstractions.HttpClients;
using TransportZone.Air.Infrastructure.Configs;
using TransportZone.Air.Infrastructure.Persistence;
using TransportZone.Air.Infrastructure.Services.Background;
using TransportZone.Air.Infrastructure.Services.HttpClients;
using TransportZone.Air.Infrastructure.Services.Messaging;
using TransportZone.Contracts.Messaging;

namespace TransportZone.Air.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
	{
		var connectionString = config.GetConnectionString("Database") ?? throw new ArgumentNullException("Database");
		services.AddDbContext<AppDbContext>(options =>
		{
			options.UseNpgsql(connectionString, o => o.UseNetTopologySuite());
		});
		services.AddHostedService<MigratorService>();
		services.AddHostedService<AirportMigratorService>();
		services.AddScoped(typeof(IRepository<>), typeof(DefaultRepository<>));
		services.AddGrpc();

		var apiNinjaConfig = config.GetSection(nameof(ApiNinjaConfig)).Get<ApiNinjaConfig>() 
		                     ?? throw new ArgumentException(nameof(ApiNinjaConfig));
		services.Configure<ApiNinjaConfig>(config);
		services.AddHttpClient<IApiNinjaClient, ApiNinjaClient>(opt =>
		{
			opt.BaseAddress = new Uri(apiNinjaConfig.Url);
			opt.DefaultRequestHeaders.Add("X-Api-Key",apiNinjaConfig.Token);
		});

		services.AddQuartz(configure =>
		{
			configure.AddJob<FlightsScheduler>(jc => jc
				.WithIdentity(nameof(FlightsScheduler)));
			configure.AddTrigger(conf => conf.ForJob(nameof(FlightsScheduler))
				.WithSimpleSchedule(
					intervalBuilder =>
					{
						intervalBuilder.WithIntervalInHours(24);
					}).StartAt(DateTimeOffset.UtcNow.AddDays(1)));
			
			configure.AddTrigger(conf => conf.ForJob(nameof(FlightsScheduler)).StartNow());
		});
		services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
		
		var kafkaOptions = config.GetSection(nameof(KafkaOptions)).Get<KafkaOptions>() 
		                   ?? throw new ArgumentNullException(nameof(KafkaOptions));
		
		services.AddMassTransit(x =>
		{
			x.UsingInMemory();
			x.AddRider(rider =>
			{
				rider.AddProducer<FlightCreatedMessage>(kafkaOptions.Produce.FlightCreatedEventsTopic);
				rider.UsingKafka((ctx, cfg) =>
				{
					cfg.Host(kafkaOptions.ConnectionString);
				});
			});
		});
		services.AddTransient<IProducer, KafkaProducer>();
		return services;
	}
}