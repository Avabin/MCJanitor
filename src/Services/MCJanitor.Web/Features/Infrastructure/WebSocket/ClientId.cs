namespace MCJanitor.Web.Features.Infrastructure.WebSocket;

public readonly record struct ClientId(int ComputerId)
{
    public static ClientId Of(int computerId) => new(computerId);
}