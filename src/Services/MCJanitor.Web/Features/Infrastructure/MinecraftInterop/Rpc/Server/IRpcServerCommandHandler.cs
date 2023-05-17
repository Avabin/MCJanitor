namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Server;

/// <summary>
/// Handles RPC commands sent from the Minecraft CC computer to the server.
/// </summary>
public interface IRpcServerCommandHandler
{
    Task<ServerCommandResult> HandleAsync(ServerCommand serverCommand, int sourceComputerId);
}