using System.Net.WebSockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Client;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Server;
using MCJanitor.Web.Features.Infrastructure.WebSocket;
using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop;

public class MinecraftComputer : IMinecraftComputer
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Formatting.None,
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    };
    private static readonly JsonSerializer _serializer = JsonSerializer.Create(_jsonSerializerSettings);
    private readonly System.Net.WebSockets.WebSocket _webSocket;
    private readonly WebSocketDataFrameReader _reader;
    private readonly IRpcServerCommandHandler _commandHandler;

    private readonly IObservable<ComputerCommandResult> _computerCommands;
    private readonly IObservable<ServerCommand> _serverCommands;
    private readonly CompositeDisposable _disposables = new();
    private bool _isConnected = false;

    public MinecraftComputer(System.Net.WebSockets.WebSocket webSocket, WebSocketDataFrameReader reader, IRpcServerCommandHandler commandHandler, int computerId)
    {
        _webSocket = webSocket;
        _reader = reader;
        _commandHandler = commandHandler;
        Id = computerId;
        
        // json is ComputerCommand if it has a ComputerId property
        _computerCommands = _reader.DataFrames.Where(x => x.Data["contract"]?.ToString() != null)
            .Select(x => x.Data.ToObject<ComputerCommandResult>(_serializer));
        
        _serverCommands = _reader.DataFrames.Where(x => x.Data["contract"]?.ToString() == null)
            .Select(x => x.Data.ToObject<ServerCommand>(_serializer));
    }
    
    public async Task StartAsync()
    {
        _disposables.Add(_serverCommands
            .Select(x => HandleServerCommand(x).ToObservable()).Concat()
            .Select(x => x.Result)
            .Select(x => SendAsync(x).ToObservable()).Concat()
            .Subscribe());
        _disposables.Add(_reader.IsConnected.Subscribe(x =>
        {
            _isConnected = x;
        }));
        await _reader.StartReadingDataFrames();
    }
    
    private async Task<ServerCommandResult> HandleServerCommand(ServerCommand command)
    {
        var result = await _commandHandler.HandleAsync(command, Id);
        return ServerCommandResult.Success(command.RequestId, result);
    }
    
    private async Task SendAsync<T>(T data)
    {
        var ms = new MemoryStream();
        var textWriter = new JsonTextWriter(new StreamWriter(ms, new UTF8Encoding()));
        _serializer.Serialize(textWriter, data);
        await textWriter.FlushAsync();
        var commandBuffer = ms.ToArray();
        await _webSocket.SendAsync(commandBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public bool IsConnected => _isConnected;
    public int Id { get; }

    /// <summary>
    /// Sends a command to the computer and waits for a response
    /// </summary>
    /// <param name="command">Command to send</param>
    /// <param name="timeout">Timeout for the command (default 5 seconds)</param>
    /// <returns>Result of the command</returns>
    /// <throws>TimeoutException if the command times out</throws>
    private async Task<ComputerCommandResult> SendCommandAsync(ComputerCommand command, TimeSpan timeout = default)
    {
        if (!_isConnected)
        {
            return ComputerCommandResult.Failure(command.RequestId, "Not connected");
        }
        timeout = timeout == default ? TimeSpan.FromSeconds(5) : timeout;
        
        var tcs = new TaskCompletionSource<ComputerCommandResult>();
        
        var subscription = _computerCommands.Where(x => x.RequestId == command.RequestId)
        .Timeout(timeout)
        .Subscribe(x =>
        {
            tcs.SetResult(x);
        });
     
        await SendAsync(command);
        var result = await tcs.Task;

        subscription.Dispose();
        
        return result;
    }

    public async Task<string> PingAsync()
    {
        var pingCommand = new ComputerCommand
        {
            RequestId = Guid.NewGuid().ToString(),
            CommandName = "ping",
            ComputerId = Id,
            Arguments = Array.Empty<string>(),
            Contract = ""
        };
        var result = await SendCommandAsync(pingCommand);
        return result.Result["message"]?.ToString() ?? "";
    }

    public async Task<IReadOnlyList<InventoryInfo>> GetInventoriesAsync()
    {
        var pingCommand = new ComputerCommand
        {
            RequestId = Guid.NewGuid().ToString(),
            CommandName = "getInventories",
            ComputerId = Id,
            Arguments = Array.Empty<string>(),
            Contract = ""
        };
        var result = await SendCommandAsync(pingCommand);
        var jObject = result.Result;
        var inventories = jObject?["inventories"]?.ToObject<List<InventoryInfo>>(_serializer);

        if (inventories == null)
        {
            throw new InvalidOperationException("Could not parse inventories");
        }
        
        return inventories;
    }

    public async Task<IReadOnlyList<SlotInfo>> GetInventoryAsync(string name)
    {
        var pingCommand = new ComputerCommand
        {
            RequestId = Guid.NewGuid().ToString(),
            CommandName = "getInventory",
            ComputerId = Id,
            Arguments = new []{name},
            Contract = ""
        };
        var result = await SendCommandAsync(pingCommand);
        var jObject = result.Result;
        
        var inventories = jObject?["slots"]?.ToObject<List<SlotInfo>>(_serializer);

        if (inventories == null)
        {
            return Array.Empty<SlotInfo>();
        }
        
        return inventories;
    }

    public async Task<SlotInfo?> GetSlotAsync(string inventoryName, int slotId)
    {
        var pingCommand = new ComputerCommand
        {
            RequestId = Guid.NewGuid().ToString(),
            CommandName = "getItemDetail",
            ComputerId = Id,
            Arguments = new []{inventoryName, slotId.ToString()},
            Contract = ""
        };
        var result = await SendCommandAsync(pingCommand);
        var jObject = result.Result;
        
        var slot = jObject?.ToObject<SlotInfo>(_serializer);

        return slot;
    }

    public async Task<int> PullItemsAsync(string source, string target, int fromSlot, int limit = 64, int? toSlot = null)
    {
        var command = new ComputerCommand
        {
            RequestId = Guid.NewGuid().ToString(),
            CommandName = "pullItems",
            ComputerId = Id,
            Arguments = new []{source, target, fromSlot.ToString(), limit.ToString(), toSlot?.ToString() ?? ""},
            Contract = ""
        };
        
        var result = await SendCommandAsync(command);
        var jObject = result.Result;
        
        
        var count = jObject?["pulled"]?.ToObject<int>() ?? 0;
        
        return count;
    }

    public async Task<int> PushItemsAsync(string source, string target, int fromSlot, int limit = 64, int? toSlot = null)
    {
        var command = new ComputerCommand
        {
            RequestId = Guid.NewGuid().ToString(),
            CommandName = "pushItems",
            ComputerId = Id,
            Arguments = new []{source, target, fromSlot.ToString(), limit.ToString(), toSlot?.ToString() ?? ""},
            Contract = ""
        };
        
        var result = await SendCommandAsync(command);
        var jObject = result.Result;
        
        var count = jObject?["pushed"]?.ToObject<int>() ?? 0;
        
        return count;
    }
}