using System.Collections.Immutable;
using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;
using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Requests;

public readonly record struct InventoriesQuery(int ComputerId) : IRequest<ImmutableList<InventoryInfo>>
{
    
}