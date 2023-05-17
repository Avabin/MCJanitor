namespace MCJanitor.Web.Features.Infrastructure.MinecraftInterop;

/// <summary>
/// Keeps track of all the computers that are connected to the server.
/// </summary>
public interface IMinecraftComputerRegistry
{
    /// <summary>
    /// Get a computer by its ID.
    /// </summary>
    /// <param name="computerId"></param>
    /// <returns>Computer if found, null otherwise.</returns>
    IMinecraftComputer? GetComputer(int computerId);
    
    /// <summary>
    /// Adds a computer to the registry.
    /// </summary>
    /// <param name="computer">Connected computer.</param>
    void AddComputer(IMinecraftComputer computer);
    
    /// <summary>
    /// Removes a computer from the registry.
    /// </summary>
    /// <param name="computerId">Computer ID.</param>
    void RemoveComputer(int computerId);
}