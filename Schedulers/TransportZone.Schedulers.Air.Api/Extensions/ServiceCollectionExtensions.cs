using MassTransit;
using Quartz;
using TransportZone.Contracts.Messaging;
using TransportZone.Schedulers.Air.Api.Consumers;
using TransportZone.Schedulers.Air.Api.Options;

namespace TransportZone.Schedulers.Air.Api.Extensions;

public static class ServiceCollectionExtensions
{
	internal static IHostBuilder AddConfigurations(this IHostBuilder host)
	{
		host.ConfigureAppConfiguration((context, config) =>
		{
			const string configurationsDirectory = "Configurations";
			var env = context.HostingEnvironment;
			config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"{configurationsDirectory}/serilog.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"{configurationsDirectory}/serilog.{env.EnvironmentName}.json", optional: true,
					reloadOnChange: true);
		});

		return host;
	}
	public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
	{
		services.AddQuartz();
		services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
		
		var kafkaOptions = config.GetSection(nameof(KafkaOptions)).Get<KafkaOptions>() 
		                   ?? throw new ArgumentNullException(nameof(KafkaOptions));
		services.Configure<KafkaOptions>(config.GetSection(nameof(KafkaOptions)));
		services.Configure<ServicesOptions>(config.GetSection(nameof(ServicesOptions)));
		services.AddMassTransit(x =>
		{
			x.AddPublishMessageScheduler();
			x.AddQuartzConsumers();
			x.AddConsumer<FlightOnTimeScheduleConsumer>();
			x.UsingRabbitMq((context, cfg) =>
			{
				cfg.Host("localhost", 5673,"/", h =>
				{
					h.Username("guest");
					h.Password("guest");
				});
				cfg.UsePublishMessageScheduler();
				cfg.ConfigureEndpoints(context);
			});
			x.AddRider(rider =>
			{
				rider.AddConsumer<FlightCreatedConsumer>();
				rider.UsingKafka((ctx, cfg) =>
				{
					cfg.Host(kafkaOptions.ConnectionString);
					cfg.TopicEndpoint<FlightCreatedMessage>(
						kafkaOptions.Consume.FlightCreatedEventsTopic,kafkaOptions.Consume.GroupId,
						e =>
						{
							e.CreateIfMissing(topic =>
							{
								topic.NumPartitions = 2;
							});
							e.ConfigureConsumer<FlightCreatedConsumer>(ctx);
						});
				});
			});
		});
		return services;
	}
}