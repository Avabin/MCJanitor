using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;
using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Requests;

public readonly record struct InventoryQuery(int ComputerId, string Name) : IRequest<IReadOnlyList<SlotInfo>>;