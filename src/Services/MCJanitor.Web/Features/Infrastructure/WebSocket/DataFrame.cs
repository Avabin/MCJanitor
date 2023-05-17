using Newtonsoft.Json.Linq;

namespace MCJanitor.Web.Features.Infrastructure.WebSocket;

/// <summary>
///    Represents a data frame sent from the Minecraft CC computer to the client.
/// </summary>
/// <param name="ComputerId">ComputerCraft computer ID.</param>
/// <param name="Data">Sent data.</param>
/// <param name="Timestamp">Timestamp of the data.</param>
public readonly record struct DataFrame(int ComputerId, JObject Data, DateTimeOffset Timestamp);