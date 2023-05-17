using System.Collections.Concurrent;

namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop;

public class MinecraftComputerRegistry : IMinecraftComputerRegistry
{
    private readonly ConcurrentDictionary<int, IMinecraftComputer> _computers = new();

    public MinecraftComputerRegistry()
    {
        
    }

    public IMinecraftComputer? GetComputer(int computerId)
    {
        if (_computers.TryGetValue(computerId, out var computer))
        {
            return computer;
        }
        
        return null;
    }

    public void AddComputer(IMinecraftComputer computer)
    {
        _computers[computer.Id] = computer;
    }

    public void RemoveComputer(int computerId)
    {
        _computers.TryRemove(computerId, out _);
    }
}