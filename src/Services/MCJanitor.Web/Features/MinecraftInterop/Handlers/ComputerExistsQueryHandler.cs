using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.MinecraftInterop.Requests;
using MediatR;

namespace MCJanitor.Web.Features.MinecraftInterop.Handlers;

public class ComputerExistsQueryHandler : ComputerHandlerBase, IRequestHandler<ComputerExistsQuery, bool>
{
    public Task<bool> Handle(ComputerExistsQuery request, CancellationToken cancellationToken)
    {
        var computer = Computer(request.ComputerId);
        return Task.FromResult(computer is not null);
    }

    public ComputerExistsQueryHandler(IMinecraftComputerRegistry computerRegistry) : base(computerRegistry)
    {
    }
}