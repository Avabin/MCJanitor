using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.MinecraftInterop.Requests;
using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;
using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Handlers;

public class SlotQueryHandler : ComputerHandlerBase, IRequestHandler<SlotQuery, SlotInfo?>
{
    public SlotQueryHandler(IMinecraftComputerRegistry computerRegistry) : base(computerRegistry)
    {
    }

    public async Task<SlotInfo?> Handle(SlotQuery request, CancellationToken cancellationToken)
    {
        var task = Computer(request.ComputerId)?.GetSlotAsync(request.Name, request.SlotIndex);

        if (task is null)
        {
            return null;
        }
        
        var slot = await task;

        return slot;
    }
}