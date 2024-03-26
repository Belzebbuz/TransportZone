namespace TransportZone.Air.Api;

internal static class Extensions
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
    
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddProblemDetails();

        return services;
    }
}