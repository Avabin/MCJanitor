using System.Collections.Immutable;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.MinecraftInterop;
using MCJanitor.Web.Features.StorageSystem.Definitions;

namespace MCJanitor.Web.Features.StorageSystem;

// key is the computer id
// where the computer id is the id of the computer that serves this storage system
public interface IStorageServerGrain : IGrainWithStringKey
{
    Task<ImmutableList<IItemContainerGrain>> GetItemContainersAsync();
}


public record StorageServerGrainState
{
    public ImmutableList<IItemContainerGrain> ItemContainers { get; init; } = ImmutableList<IItemContainerGrain>.Empty;
}
public class StorageServerGrain : Grain, IStorageServerGrain
{
    private readonly IMinecraftComputerRegistry _computerRegistry;
    private int _computerId;

    private bool ValidateKey()
    {
        if (!int.TryParse(this.GetPrimaryKeyString(), out var computerId)) return false;
        _computerId = computerId;
        return true;

    }

    public StorageServerGrain(IMinecraftComputerRegistry computerRegistry)
    {
        _computerRegistry = computerRegistry;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!ValidateKey())
        {
            throw new InvalidOperationException("Key must be a valid computer id");
        }
        var maybeComputer = _computerRegistry.GetComputer(_computerId);
        if (maybeComputer is null)
        {
            throw new ComputerNotFoundException(_computerId);
        }
        return base.OnActivateAsync(cancellationToken);
    }

    public async Task<ImmutableList<IItemContainerGrain>> GetItemContainersAsync()
    {
        throw new NotImplementedException();
    }
}

public class ComputerNotFoundException : Exception
{
    public ComputerNotFoundException(int computerId) : base($"Computer with id {computerId} not found")
    {
    }
}