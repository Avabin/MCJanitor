using System.Collections.Immutable;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.MinecraftInterop.Requests;
using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;
using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Handlers;

public class InventoriesQueryHandler : ComputerHandlerBase, IRequestHandler<InventoriesQuery,ImmutableList<InventoryInfo>>
{
    public InventoriesQueryHandler(IMinecraftComputerRegistry computerRegistry) : base(computerRegistry)
    {
    }

    public async Task<ImmutableList<InventoryInfo>> Handle(InventoriesQuery request, CancellationToken cancellationToken)
    {
        var computer = Computer(request.ComputerId);
        if (computer is null)
        {
            return ImmutableList<InventoryInfo>.Empty;
        }

        var inventories = await computer.GetInventoriesAsync();
        return inventories.ToImmutableList();
    }
}