using System.Collections.Immutable;
using MCJanitor.Web.Features.MinecraftInterop;
using MCJanitor.Web.Features.StorageSystem.Definitions;
using MCJanitor.Web.Features.StorageSystem.Entities;

namespace MCJanitor.Web.Features.StorageSystem.AggregateRoots;

[GenerateSerializer, Immutable]
public readonly record struct StorageSystem(
    [property: Id(0)] StorageSystemKey Id,
    [property: Id(2)] IStorageServerGrain? Server) : IAggregateRoot<StorageSystemKey>
{
    public async Task<ImmutableList<IItemContainerGrain>> GetItemContainersAsync()
    {
        if (Server is null)
        {
            return ImmutableList<IItemContainerGrain>.Empty;
        }

        return await Server.GetItemContainersAsync();
    }
}

[GenerateSerializer, Immutable]
public readonly record struct StorageSystemKey([property: Id(0)] Guid Id) : IValueObject;