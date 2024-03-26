namespace TransportZone.Air.Infrastructure.Configs;

internal sealed class KafkaOptions
{
    public required string ConnectionString { get; set; }
    public required Produce Produce { get; set; }
}

internal sealed class Produce
{
    public required string FlightCreatedEventsTopic { get; set; }
}

