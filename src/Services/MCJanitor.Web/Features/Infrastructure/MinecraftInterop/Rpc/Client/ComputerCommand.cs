namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Client;

public readonly record struct ComputerCommand(string RequestId, string CommandName, string[] Arguments, int ComputerId,
    string Contract = "");