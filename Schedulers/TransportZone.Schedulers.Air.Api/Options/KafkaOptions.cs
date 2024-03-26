namespace TransportZone.Schedulers.Air.Api.Options;

internal sealed class KafkaOptions
{
	public required string ConnectionString { get; set; }
	public required Consume Consume { get; set; }
}


internal sealed class Consume
{
	public required string GroupId { get; set; }
	public required string FlightCreatedEventsTopic { get; set; }
}
