namespace TransportZone.Contracts.Messaging;

public interface IMessage;
public record FlightCreatedMessage(int Id) : IMessage;
