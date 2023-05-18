using System.Collections.Immutable;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.MinecraftInterop;
using MCJanitor.Web.Features.MinecraftInterop.Requests;
using MCJanitor.Web.Features.StorageSystem.Definitions;
using MCJanitor.Web.Features.StorageSystem.Entities;
using MediatR;

namespace MCJanitor.Web.Features.StorageSystem;

// key is the computer id
// where the computer id is the id of the computer that serves this storage system
public interface IStorageServerGrain : IGrainWithIntegerKey
{
    Task RefreshAsync();
    Task<ImmutableList<IItemContainerGrain>> GetItemContainersAsync();
}


public record StorageServerGrainState
{
    public ImmutableList<IItemContainerGrain> ItemContainers { get; init; } = ImmutableList<IItemContainerGrain>.Empty;
}
public class StorageServerGrain : Grain<StorageServerGrainState>, IStorageServerGrain
{
    private readonly IMediator _mediator;
    private int _computerId;

    private bool ValidateKey()
    {;
        _computerId = (int)this.GetPrimaryKeyLong();
        return true;

    }

    public StorageServerGrain(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        if (!ValidateKey())
        {
            throw new InvalidOperationException("Key must be a valid computer id");
        }
        var hasBeenSeen = await _mediator.Send(new ComputerExistsQuery(_computerId), cancellationToken);
        if (!hasBeenSeen)
        {
            throw new ComputerNotFoundException(_computerId);
        }
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task RefreshAsync()
    {
        // Fetch the inventories from the computer
        var inventories = await _mediator.Send(new InventoriesQuery(_computerId));
        var grains = inventories
            .Select(x => ItemContainerKey.Of(_computerId, x.Name))
            .Select(x => GrainFactory.GetGrain<IItemContainerGrain>(x));
        
        State = State with { ItemContainers = grains.ToImmutableList() };
        
        // refresh the inventories
        foreach (var grain in grains)
        {
            await grain.RefreshAsync();
        }
        
        await WriteStateAsync();
    }

    public Task<ImmutableList<IItemContainerGrain>> GetItemContainersAsync() => Task.FromResult(State.ItemContainers);
}

public class ComputerNotFoundException : Exception
{
    public ComputerNotFoundException(int computerId) : base($"Computer with id {computerId} not found")
    {
    }
}