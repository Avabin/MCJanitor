using System.Net.WebSockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using MCJanitor.Web.Controllers;
using Newtonsoft.Json.Linq;

namespace MCJanitor.Web.Features.Infrastructure.WebSocket;

/// <summary>
/// Reads data frames from the WebSocket.
/// </summary>
public class WebSocketDataFrameReader
{
    private readonly System.Net.WebSockets.WebSocket _webSocket;
    private readonly ClientId _clientId; // computer id
    private readonly ILogger<WebSocketDataFrameReader> _logger;
    private readonly ISubject<DataFrame> _dataFrames;
    public IObservable<DataFrame> DataFrames { get; private set; }
    private readonly ISubject<bool> _isConnected;
    public IObservable<bool> IsConnected => _isConnected.AsObservable();

    public WebSocketDataFrameReader(System.Net.WebSockets.WebSocket webSocket, ClientId clientId, ILogger<WebSocketDataFrameReader> logger)
    {
        _webSocket = webSocket;
        _clientId = clientId;
        _logger = logger;
        
        _dataFrames = new Subject<DataFrame>();
        _isConnected = new BehaviorSubject<bool>(false);


        DataFrames = Observable.Create<DataFrame>(observer =>
        {
            var compositeDisposable = new CompositeDisposable();
            var subscription = _dataFrames.Subscribe(observer);
            compositeDisposable.Add(subscription);
            return compositeDisposable;
        });
    }

    public async Task StartReadingDataFrames(CancellationToken cancellationToken = default)
    {
        _isConnected.OnNext(true);
        var buffer = new byte[1024];
        var msgbuffer = new StringBuilder();

        while (!_webSocket.CloseStatus.HasValue && !cancellationToken.IsCancellationRequested && _webSocket.State is WebSocketState.Open or WebSocketState.Connecting)
        {
            try
            {
                if (await HandleWebSocket(cancellationToken, buffer, msgbuffer)) continue;
            }
            catch (Exception ex)
            {
                _isConnected.OnNext(false);
                _logger.LogError(ex, "Error while reading command from WebSocket");
            }
        }
        _isConnected.OnNext(false);
    }

    // returns true if the message is not complete
    private async Task<bool> HandleWebSocket(CancellationToken cancellationToken, byte[] buffer, StringBuilder msgbuffer)
    {
        var receiveBuffer = new Memory<byte>(buffer);
        var result = await _webSocket.ReceiveAsync(receiveBuffer, cancellationToken);

        switch (result.MessageType)
        {
            case WebSocketMessageType.Text:
            {
                var fragment = Encoding.UTF8.GetString(receiveBuffer[..result.Count].ToArray());
                msgbuffer.Append(fragment);
                
                if (!result.EndOfMessage) // message is not complete
                    return true;

                HandleTextAsync(msgbuffer.ToString());
                msgbuffer.Clear();
                break;
            }
            case WebSocketMessageType.Close:
            {
                await HandleCloseAsync();
                break;
            }
            case WebSocketMessageType.Binary:
                _logger.LogWarning("Received binary message from WebSocket");
                break;
            default:
                throw new ArgumentOutOfRangeException("result.MessageType", "Unknown WebSocketMessageType");
        }

        return false;
    }

    private void HandleTextAsync(string receive)
    {
        var requestTime = DateTimeOffset.UtcNow;
        var jobject = JObject.Parse(receive);
        var dataFrame = new DataFrame(_clientId.ComputerId, jobject, requestTime);

        _dataFrames.OnNext(dataFrame);
    }

    private async Task HandleCloseAsync()
    {
        _logger.LogInformation("Received close message from WebSocket");
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }
}