using System.Collections.Immutable;
using MCJanitor.Web.Features.StorageSystem.ValueObjects;

namespace MCJanitor.Web.Features.StorageSystem.Definitions;

public interface IItemContainerGrain : IGrainWithStringKey
{
    Task<string> GetContainerName();
    Task RefreshAsync();
    Task<int> GetContainerSize();
    Task<ImmutableList<Slot>> ListItems();
    Task<DetailedItemInfo> GetItemDetails(int slotId);
    Task<int> GetSlotCapacity(int slotId);
    Task<int> MoveItemTo(IItemContainerGrain targetContainer, int fromSlotId, int quantity = 64, int? toSlotId = null);
    Task<int> MoveItemFrom(IItemContainerGrain sourceContainer, int fromSlotId, int quantity = 64, int? toSlotId = null);
}