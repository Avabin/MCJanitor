using MCJanitor.Web.Features.MinecraftInterop.ValueObjects;
using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Requests;

public readonly record struct SlotQuery(int ComputerId, string Name, int SlotIndex) : IRequest<SlotInfo?>;