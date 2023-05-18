local rpc = {}

local function HandleGetFullInventory(name)
    local periph = peripheral.wrap(name)
    local size = periph.size()

    local result = { name = name, size = size, slots = {} }

    for slot = 1, size, 1 do
        local slotDetails = periph.getItemDetail(slot)
        table.insert(result.slots, slotDetails)
    end

    return result
end

local function HandleGetSlotDetails(name, slot)
    local periph = peripheral.wrap(name)
    local item = periph.getItemDetail(slot)
    return { slotIndex = slot, item = periph.getItemDetail(slot) }
end

local function HandleGetInventories()
    local inventories = { peripheral.find("inventory") }
    local result = { inventories = {} }
    for _, value in ipairs(inventories) do
        local name = peripheral.getName(value)

        table.insert(result.inventories, { name = name, size = value.size() })
    end
    return result
end

    local function HandleGetInventory(name)
        local inv = peripheral.wrap(name)
        if inv == nil then
            return {}
        end
        local slots = inv.list()
        local result = { slots = {}, name = name }
        local hasItems = false
        for i, slot in pairs(slots) do
            hasItems = true
            table.insert(result.slots, { slotIndex = i, item = slot })
        end
        if not hasItems then
            result.slots = textutils.empty_json_array
        end
        return result
    end

    local function HandlePullItems(source, target, fromSlot, limit, toSlot)
        local sourceInv = peripheral.wrap(source)

        local moved = sourceInv.pullItems(target, fromSlot, limit, toSlot)

        return { pulled = moved }
    end

    local function HandlePushItems(source, target, fromSlot, limit, toSlot)
        local sourceInv = peripheral.wrap(source)

        local moved = sourceInv.pushItems(target, fromSlot, limit, toSlot)

        return { pushed = moved }
    end

    function rpc.Validate(json)
        local command = json
        if command == nil then
            print("Cannot deserialize: " .. json)
        end
        local isValid = true
        if command == nil then
            return false
        end
        if command.requestId == nil then
            return false
        end
        if command.commandName == nil then
            return false
        end
        if command.arguments == nil then
            return false
        end
        if (command.computerId == nil) then
            return false
        end

        return true
    end

    local function Ok(requestId, contract, result)
        return { requestId = requestId, errorMessage = "", result = result, contract = contract }
    end
    local function Error(requestId, error, contract)
        return { requestId = requestId, errorMessage = error, result = {}, contract = contract }
    end
    local function WrongId(requestId, contract)
        return Error(requestId, "wrong_computer_id", contract)
    end
    local function UnknownCommand(requestId, contract)
        return Error(requestId, "unknown_command", contract)
    end

    function rpc.HandleCommand(command)
        local requestId = command.requestId
        local commandName = command.commandName
        local arguments = command.arguments
        local computerId = command.computerId
        local contract = command.contract
        local thisComputerId = os.getComputerID()
        if thisComputerId == computerId then
            print("Received command for this computer: " .. commandName .. " (" .. requestId .. ")")
            if commandName == "ping" then
                return Ok(requestId, contract, { message = "pong" })
            elseif commandName == "getInventories" then
                local inventories = HandleGetInventories()
                return Ok(requestId, contract, inventories)
            elseif commandName == "getInventory" then
                local invName = arguments[1]
                return Ok(requestId, contract, HandleGetInventory(invName))
            elseif commandName == "getInventoryDetails" then
                local invName = arguments[1]
                return Ok(requestId, contract, HandleGetFullInventory(invName))
            elseif commandName == "getItemDetail" then
                local invName = arguments[1]
                local slotNumber = tonumber(arguments[2])
                return Ok(requestId, contract, HandleGetSlotDetails(invName, slotNumber))
            elseif commandName == "pullItems" then
                local source = arguments[1]
                local target = arguments[2]
                local fromSlot = tonumber(arguments[3])
                local limit = tonumber(arguments[4])
                local toSlot = arguments[5]
                if toSlot == "" then
                    toSlot = nil
                else
                    toSlot = tonumber(toSlot)
                end
                return Ok(requestId, contract, HandlePullItems(source, target, fromSlot, limit, toSlot))
            elseif commandName == "pushItems" then
                local source = arguments[1]
                local target = arguments[2]
                local fromSlot = tonumber(arguments[3])
                local limit = tonumber(arguments[4])
                local toSlot = arguments[5]
                if toSlot == "" then
                    toSlot = nil
                else
                    toSlot = tonumber(toSlot)
                end
                return Ok(requestId, contract, HandlePushItems(source, target, fromSlot, limit, toSlot))
            end
            return UnknownCommand(requestId, contract)
        else
            return WrongId(requestId, contract)
        end
    end

return rpc
