using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;

namespace MCJanitor.Web.Features.MinecraftInterop.Handlers;

public abstract class ComputerHandlerBase
{
    private readonly IMinecraftComputerRegistry _computerRegistry;

    protected ComputerHandlerBase(IMinecraftComputerRegistry computerRegistry)
    {
        _computerRegistry = computerRegistry;
    }
    
    protected IMinecraftComputer? Computer(int computerId) => _computerRegistry.GetComputer(computerId);
}