using System.ComponentModel.DataAnnotations;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.MinecraftInterop;
using Microsoft.AspNetCore.Mvc;

namespace MCJanitor.Web.Controllers;

[ApiController, Route("api/[controller]")]
public class MinecraftComputerController : ControllerBase
{
    private readonly IMinecraftComputerRegistry _computerRegistry;


    public MinecraftComputerController( IMinecraftComputerRegistry computerRegistry)
    {
        _computerRegistry = computerRegistry;
    }
    
    // send ping command
    [HttpPost("{computerId:int}/command/ping")]
    public async Task<IActionResult> PingAsync(int computerId)
    {
        var minecraftComputer = _computerRegistry.GetComputer(computerId);
        if (minecraftComputer == null)
        {
            return NotFound("Computer not connected");
        }
        var pongString = await minecraftComputer.PingAsync();
        return Ok(pongString);
    }
    
    // get inventories
    [HttpGet("{computerId:int}/inventories")]
    public async Task<IActionResult> GetInventoriesAsync(int computerId)
    {
        var minecraftComputer = _computerRegistry.GetComputer(computerId);
        if (minecraftComputer == null)
        {
            return NotFound("Computer not connected");
        }
        var inventories = await minecraftComputer.GetInventoriesAsync();
        return Ok(inventories);
    }
    
    // get inventory content by name
    [HttpGet("{computerId:int}/inventory/{inventoryName}")]
    public async Task<IActionResult> GetInventoryAsync(int computerId, string inventoryName)
    {
        var minecraftComputer = _computerRegistry.GetComputer(computerId);
        if (minecraftComputer == null)
        {
            return NotFound("Computer not connected");
        }
        var inventory = await minecraftComputer.GetInventoryAsync(inventoryName);
        return Ok(inventory);
    }
    
    // get slot details
    [HttpGet("{computerId:int}/inventory/{inventoryName}/{slotId:int}")]
    public async Task<IActionResult> GetSlotAsync(int computerId, string inventoryName, int slotId)
    {
        var minecraftComputer = _computerRegistry.GetComputer(computerId);
        if (minecraftComputer == null)
        {
            return NotFound("Computer not connected");
        }
        var slot = await minecraftComputer.GetSlotAsync(inventoryName, slotId);
        
        if (!slot.HasValue)
        {
            return NotFound("Slot not found");
        }
        return Ok(slot);
    }
    
    // pull items from one inventory to another
    [HttpPost("{computerId:int}/inventory/{sourceInventoryName}/{targetInventoryName}/pull")]
    public async Task<IActionResult> PullItemsAsync(int computerId, string sourceInventoryName, string targetInventoryName, [FromQuery] PullItemsRequest request)
    {
        int limit = request.Limit == 0 ? 64 : request.Limit;
        var minecraftComputer = _computerRegistry.GetComputer(computerId);
        if (minecraftComputer == null)
        {
            return NotFound("Computer not connected");
        }
        var pulledItems = await minecraftComputer.PullItemsAsync(sourceInventoryName, targetInventoryName, request.FromSlot, limit, request.ToSlot);
        return Ok(pulledItems);
    }
    
    // push items from one inventory to another
    [HttpPost("{computerId:int}/inventory/{sourceInventoryName}/{targetInventoryName}/push")]
    public async Task<IActionResult> PushItemsAsync(int computerId, string sourceInventoryName, string targetInventoryName, [FromQuery] PushItemsRequest request)
    {
        int limit = request.Limit == 0 ? 64 : request.Limit;
        var minecraftComputer = _computerRegistry.GetComputer(computerId);
        if (minecraftComputer == null)
        {
            return NotFound("Computer not connected");
        }
        var pushedItems = await minecraftComputer.PushItemsAsync(sourceInventoryName, targetInventoryName, request.FromSlot, limit, request.ToSlot);
        return Ok(pushedItems);
    }
}

public record PullItemsRequest([Required] [FromQuery] int FromSlot, [FromQuery] int Limit = 64, [FromQuery] int? ToSlot = null)
{
    public PullItemsRequest() : this(0, 0, null)
    {
    }
}

public record PushItemsRequest : PullItemsRequest
{
}