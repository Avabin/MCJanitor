using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.MinecraftInterop.Requests;
using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Handlers;

public class IsOnlineHandler : ComputerHandlerBase, IRequestHandler<IsOnlineQuery, bool>
{
    public async Task<bool> Handle(IsOnlineQuery request, CancellationToken cancellationToken) => 
        Computer(request.ComputerId)?.IsConnected ?? false;

    public IsOnlineHandler(IMinecraftComputerRegistry computerRegistry) : base(computerRegistry)
    {
    }
}