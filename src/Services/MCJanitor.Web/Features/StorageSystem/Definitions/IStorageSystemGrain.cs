using System.Collections.Immutable;
using MCJanitor.Web.Features.MinecraftInterop;

namespace MCJanitor.Web.Features.StorageSystem.Definitions;

public interface IStorageSystemGrain : IGrainWithGuidKey
{
    Task SetMinecraftComputerServer(IStorageServerGrain? computerGrain);
    Task<ImmutableList<IItemContainerGrain>> GetItemContainers();
}
