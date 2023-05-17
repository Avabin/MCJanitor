local rpc = require("rpc")

local client_header = "X-ClientId"
Client = nil
local host_url = "ws://10.0.0.5:5228/ws"
All = {"0","1","2","3","4","5","6","7","8","9","a","b","c","d","e","f","g", "h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z","A","B","C","D","E","F","G","H","I","J",  "K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z","!","#","$","%","&","/","(",")","="}
function randstr(lenght, from)
    local str=""    
    from = from or All
    for i=1, lenght do str = str..from[math.random(1, #All)] end
    return str
  end

local function ReadSystemStorageName()
    -- check if the file exists
    if fs.exists("/.system_storage_name") then
        local file = fs.open("/.system_storage_name", "r")
        local name = file.readLine()
        file.close()
        return name
    end
    write("Enter system storage name: ")
    local name = read()
    -- verify that the name has only a-z, A-Z, 0-9, -, and _
    if name:match("[^%w%-_]") then
        print("Invalid name")
        return ReadSystemStorageName()
    end
    -- write the name to the file
    local file = fs.open("/.system_storage_name", "w")
    file.writeLine(name)
    file.close()
end
local function HandleWebSocketSuccess(event, url, handle)
    print("Succcess connecting to " .. url)
    print(handle)
    Client = handle
    
end

local function HandleWebSocketFailure(event, url, err)
    print("Error: " .. err .. " in connection with url " .. url)
end

local function HandleWebSocketMessage(event, url, message)
    print("Message: " .. message .. " from " .. url)
    local json = textutils.unserialiseJSON(message)
    if json == nil then
        print("not json message: "..message)
    end
    local isCommand = rpc.Validate(json)
    if isCommand then
        print("Raising local rpc event")
        os.queueEvent("local_rpc", json)
    end
end

local function HandleCommandRpc(event, command)
    local msg = textutils.serializeJSON(command)
    Client.send(msg)
end

local name = ReadSystemStorageName()
local id = os.computerID()
ClientName = id

local function main()
    http.websocketAsync(host_url.."?clientId="..id)
    while true do
        local eventData = {os.pullEventRaw()}
        local event = eventData[1]
        if event == "terminate" then
            print("Termination")
            if Client ~= nil then
                Client.close()
            end
            break
        elseif event == "websocket_success" then
            HandleWebSocketSuccess(event, eventData[2], eventData[3])
        elseif event == "websocket_failure" then
            HandleWebSocketFailure(event, eventData[2], eventData[3])
            break
        elseif event == "websocket_message" then
            HandleWebSocketMessage(event, eventData[2], eventData[3])
        elseif event == "rpc" then
            HandleCommandRpc(event, eventData[2])
        elseif event == "local_rpc" then
            print("Handling rpc command")
            local command = eventData[2]
            local result = rpc.HandleCommand(command)
            local json = textutils.serializeJSON(result)
            Client.send(json)
        end
    end
end

-- MainC = coroutine.create(main)

main()
