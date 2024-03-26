using Serilog;
using TransportZone.Air.Api;
using TransportZone.Air.Application.Extensions;
using TransportZone.Air.Infrastructure.Extensions;
using TransportZone.Air.Infrastructure.Services.Grps;

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
try
{
	var builder = WebApplication.CreateBuilder(args);
	builder.Host.AddConfigurations();
	builder.Host.UseSerilog((_, config) =>
	{
		config.WriteTo.Console()
			.ReadFrom.Configuration(builder.Configuration);
	});
	builder.Services.AddInfrastructure(builder.Configuration);
	builder.Services.AddPresentation();
	builder.Services.AddApplication();
	
	var app = builder.Build();
	app.MapGrpcService<GrpsFlightService>();
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}
	app.MapControllers();
	app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("StopTheHostException", StringComparison.Ordinal))
{
	StaticLogger.EnsureInitialized();
	Log.Fatal(ex, "Unhandled exception");
}
finally
{
	StaticLogger.EnsureInitialized();
	Log.Information("Server Shutting down...");
	Log.CloseAndFlush();
}
