using MCJanitor.Web.Controllers;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Server;
using Microsoft.AspNetCore.Mvc;

namespace MCJanitor.Web.Features.Infrastructure.WebSocket;

[ApiController, Route("/ws")]
public partial class WebSocketController : ControllerBase
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly IRpcServerCommandHandler _commandHandler;
    private readonly ILogger<WebSocketController> _logger;
    private readonly IMinecraftComputerRegistry _computerRegistry;

    public WebSocketController(ILoggerFactory loggerFactory, IRpcServerCommandHandler commandHandler, ILogger<WebSocketController> logger, IMinecraftComputerRegistry computerRegistry)
    {
        _loggerFactory = loggerFactory;
        _commandHandler = commandHandler;
        _logger = logger;
        _computerRegistry = computerRegistry;
    }
    [HttpGet]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            // clientId is a int that identifies the connecting computer
            // get clientId from query
            var clientIdString = HttpContext.Request.Query["clientId"].FirstOrDefault();
            if (string.IsNullOrEmpty(clientIdString))
            {
                _logger.LogError("X-ClientId header is required");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync("X-ClientId header is required");
                return;
            }
            // validate clientId
            if (!int.TryParse(clientIdString, out var clientId))
            {
                _logger.LogError("X-ClientId header is invalid");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync("X-ClientId header is invalid");
                return;
            }
            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            
            var logger = _loggerFactory.CreateLogger<WebSocketDataFrameReader>();
            _logger.LogInformation("WebSocket connection accepted for client {ClientId}", clientId);
            var clientIdStruct = ClientId.Of(clientId);
            var dataReader = new WebSocketDataFrameReader(webSocket, clientIdStruct, logger);
            _logger.LogInformation("Starting to read commands for client {ClientId}", clientId);
            var computer = new MinecraftComputer(webSocket, dataReader,_commandHandler, clientId);
            _computerRegistry.AddComputer(computer);
            await computer.StartAsync();
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}