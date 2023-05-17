namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Server;

public class RpcServerCommandHandler : IRpcServerCommandHandler
{
    public async Task<ServerCommandResult> HandleAsync(ServerCommand serverCommand, int sourceComputerId)
    {
        switch (serverCommand.CommandName)
        {
            case "ping":
                return ServerCommandResult.Success(serverCommand.RequestId, "pong");
        }
        
        return ServerCommandResult.Failure(serverCommand.RequestId, "Unknown command");
    }
}