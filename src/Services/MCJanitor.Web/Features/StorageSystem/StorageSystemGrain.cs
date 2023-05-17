using System.Collections.Immutable;
using MCJanitor.Web.Features.StorageSystem.Definitions;

namespace MCJanitor.Web.Features.StorageSystem;

public class StorageSystemGrain : Grain<AggregateRoots.StorageSystem>, IStorageSystemGrain
{
    public async Task SetMinecraftComputerServer(IStorageServerGrain? computerGrain)
    {
        if (State.Server is not null)
        {
            return;
        }
        
        State = State with { Server = computerGrain };
        await WriteStateAsync();
    }

    public async Task<ImmutableList<IItemContainerGrain>> GetItemContainers() => 
        await State.GetItemContainersAsync();
        
}