using System.Collections.Immutable;
using MCJanitor.Web.Features.StorageSystem.Definitions;
using MCJanitor.Web.Features.StorageSystem.Entities;
using MCJanitor.Web.Features.StorageSystem.ValueObjects;

namespace MCJanitor.Web.Features.StorageSystem;

public class ItemContainerGrain : Grain<ItemContainer>, IItemContainerGrain
{
    public Task<int> GetContainerSize() => Task.FromResult(State.Slots.Count);

    public Task<ImmutableList<Slot>> ListItems() =>
        Task.FromResult(State.Slots.Where(slot => slot.ItemStack.CurrentStack > 0).ToImmutableList());

    public Task<DetailedItemInfo> GetItemDetails(int slotId)
    {
        // Implement your logic here
        throw new NotImplementedException();
    }

    public Task<int> GetSlotCapacity(int slotId) => Task.FromResult(State.Slots[slotId].SlotCapacity);

    public async Task<int> MoveItemTo(IItemContainerGrain targetContainer, int sourceSlotId, int quantity = 64, int? targetSlotId = null)
    {
        // Implement your logic here
        throw new NotImplementedException();
    }

    public async Task<int> MoveItemFrom(IItemContainerGrain sourceContainer, int targetSlotId, int quantity = 64, int? sourceSlotId = null)
    {
        // Implement your logic here
        throw new NotImplementedException();
    }
}