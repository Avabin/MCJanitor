namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Server;

/// <summary>
/// Represents a RPC command result sent from the server to the Minecraft CC computer.
/// </summary>
/// <param name="RequestId">Randomly generated request ID.</param>
/// <param name="ErrorMessage">Error message if the command failed.</param>
/// <param name="Result">Command result.</param>
public record ServerCommandResult(string RequestId, string ErrorMessage, object Result)
{
    public static ServerCommandResult Success(string requestId, object result) => new(requestId, "", result);
    public static ServerCommandResult Failure(string requestId, string errorMessage) => new(requestId, errorMessage, new {});
}