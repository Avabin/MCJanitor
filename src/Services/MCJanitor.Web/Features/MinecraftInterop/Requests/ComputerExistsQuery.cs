using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Requests;

public readonly record struct ComputerExistsQuery(int ComputerId) : IRequest<bool>
{
    
}