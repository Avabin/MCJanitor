using System.Collections.Immutable;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.MinecraftInterop.Requests;
using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;
using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Handlers;

public class InventoryQueryHandler : ComputerHandlerBase, IRequestHandler<InventoryQuery, IReadOnlyList<SlotInfo>>
{
    public InventoryQueryHandler(IMinecraftComputerRegistry computerRegistry) : base(computerRegistry)
    {
    }

    public async Task<IReadOnlyList<SlotInfo>> Handle(InventoryQuery request, CancellationToken cancellationToken)
    {
        var task = Computer(request.ComputerId)?.GetInventoryAsync(request.Name);

        if (task is null)
        {
            return ImmutableList<SlotInfo>.Empty;
        }
        
        var inventory = await task;

        return inventory;
    }
}