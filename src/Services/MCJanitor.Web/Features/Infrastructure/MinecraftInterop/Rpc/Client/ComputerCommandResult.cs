using Newtonsoft.Json.Linq;

namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Client;

public readonly record struct ComputerCommandResult(string RequestId, string ErrorMessage, JObject Result, string Contract = "")
{
    public static ComputerCommandResult Failure(string commandRequestId, string errorMessage) =>
        new(commandRequestId, errorMessage, new JObject(), "");
}