namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Server;

/// <summary>
/// Represents a RPC command sent from the Minecraft CC computer to the server.
/// </summary>
/// <param name="RequestId">Randomly generated request ID.</param>
/// <param name="CommandName">Command name.</param>
/// <param name="Arguments">Command arguments.</param>
public readonly record struct ServerCommand(string RequestId, string CommandName, string[] Arguments);