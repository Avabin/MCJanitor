using MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Client;
using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Commands;

public readonly record struct PullItemsCommand(int ComputerId, string Source, string Target, int SlotId,
    int Amount = 64, int? DestinationSlotId = null, string Contract = "") : IRequest<int>;