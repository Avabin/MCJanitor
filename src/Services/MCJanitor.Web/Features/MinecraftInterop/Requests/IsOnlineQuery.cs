using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Requests;

public readonly record struct IsOnlineQuery(int ComputerId) : IRequest<bool>;