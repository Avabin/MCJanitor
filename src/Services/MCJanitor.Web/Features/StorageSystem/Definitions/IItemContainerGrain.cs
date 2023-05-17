using System.Collections.Immutable;
using MCJanitor.Web.Features.StorageSystem.ValueObjects;

namespace MCJanitor.Web.Features.StorageSystem.Definitions;

public interface IItemContainerGrain : IGrainWithStringKey
{
    Task<int> GetContainerSize();
    Task<ImmutableList<Slot>> ListItems();
    Task<DetailedItemInfo> GetItemDetails(int slotId);
    Task<int> GetSlotCapacity(int slotId);
    Task<int> MoveItemTo(IItemContainerGrain targetContainer, int sourceSlotId, int quantity = 64, int? targetSlotId = null);
    Task<int> MoveItemFrom(IItemContainerGrain sourceContainer, int targetSlotId, int quantity = 64, int? sourceSlotId = null);
}