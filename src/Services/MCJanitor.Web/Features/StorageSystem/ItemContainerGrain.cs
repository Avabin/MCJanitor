using System.Collections.Immutable;
using MCJanitor.Web.Features.MinecraftInterop.Commands;
using MCJanitor.Web.Features.MinecraftInterop.Requests;
using MCJanitor.Web.Features.StorageSystem.Definitions;
using MCJanitor.Web.Features.StorageSystem.Entities;
using MCJanitor.Web.Features.StorageSystem.ValueObjects;
using MediatR;

namespace MCJanitor.Web.Features.StorageSystem;

public class ItemContainerGrainState
{
    public ItemContainer Container { get; set; } = ItemContainer.Empty;
}
public class ItemContainerGrain : Grain<ItemContainer>, IItemContainerGrain
{
    private readonly IMediator _mediator;
    private int _computerId;
    private string _inventoryId;

    public ItemContainerGrain(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        ItemContainerKey key = this.GetPrimaryKeyString();
        (_computerId, _inventoryId) = key;
        return base.OnActivateAsync(cancellationToken);
    }

    public Task<string> GetContainerName() => 
        Task.FromResult(_inventoryId);

    public async Task RefreshAsync()
    {
        // Query the computer for the inventory
        var inventory = await _mediator.Send(new InventoryQuery(_computerId, _inventoryId));

        var container = ItemContainer.FromSlotEnumerable(this.GetPrimaryKeyString(), inventory);
        
        State = container;
        await WriteStateAsync();
    }

    public Task<int> GetContainerSize() => Task.FromResult(State.Slots.Count);

    public Task<ImmutableList<Slot>> ListItems() =>
        Task.FromResult(State.Slots.Where(slot => slot.ItemStack.Count > 0).ToImmutableList());

    public async Task<DetailedItemInfo> GetItemDetails(int slotId)
    {
        // Implement your logic here
        throw new NotImplementedException();
    }

    public Task<int> GetSlotCapacity(int slotId) => Task.FromResult(State.Slots[slotId].SlotIndex);

    public async Task<int> MoveItemTo(IItemContainerGrain targetContainer, int fromSlotId, int quantity = 64, int? toSlotId = null)
    {
        ItemContainerKey targetContainerKey = targetContainer.GetPrimaryKeyString();
        var targetContainerName = targetContainerKey.PeripheralName;
        
        var command = new PushItemsCommand(_computerId, _inventoryId, targetContainerName, fromSlotId, quantity, toSlotId);
        
        var result = await _mediator.Send(command);
        
        if (result > 0)
        {
            await RefreshAsync();
            await targetContainer.RefreshAsync();
        }
        
        return result;
    }

    public async Task<int> MoveItemFrom(IItemContainerGrain sourceContainer, int fromSlotId, int quantity = 64, int? toSlotId = null)
    {
        ItemContainerKey sourceContainerKey = sourceContainer.GetPrimaryKeyString();
        var sourceContainerName = sourceContainerKey.PeripheralName;
        
        var command = new PushItemsCommand(_computerId, sourceContainerName, _inventoryId, fromSlotId, quantity, toSlotId);
        
        var result = await _mediator.Send(command);
        
        if (result > 0)
        {
            await RefreshAsync();
            await sourceContainer.RefreshAsync();
        }
        
        return result;
    }
}