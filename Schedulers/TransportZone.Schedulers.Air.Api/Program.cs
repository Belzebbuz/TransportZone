using Serilog;
using TransportZone.Schedulers.Air.Api;
using TransportZone.Schedulers.Air.Api.Extensions;

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
	builder.Services.AddServices(builder.Configuration);
	var app = builder.Build();
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
