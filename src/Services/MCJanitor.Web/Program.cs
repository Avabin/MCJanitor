using LiteDB.Async;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop;
using MCJanitor.Web.Features.Infrastructure.MinecraftInterop.Rpc.Server;
using Microsoft.AspNetCore.WebSockets;
using Orleans.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseOrleans(siloBuilder => 
    siloBuilder.UseLocalhostClustering().Configure<ClusterOptions>(options =>
    {
        options.ClusterId = "MCJanitor";
        options.ServiceId = "MCJanitor.Silo";
    }).AddMemoryGrainStorageAsDefault());
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(configuration =>
    {
        configuration.RegisterServicesFromAssemblyContaining<Program>();
    })
    .AddMemoryCache()
    .AddSingleton<ILiteDatabaseAsync>(provider =>
    {
        var config = provider.GetRequiredService<IConfiguration>();
        var liteDbConnectionString = config.GetConnectionString("LiteDb");
        return new LiteDatabaseAsync(liteDbConnectionString);
    })
    .AddTransient<IRpcServerCommandHandler, RpcServerCommandHandler>()
    .AddSingleton<IMinecraftComputerRegistry, MinecraftComputerRegistry>();

builder.Services.AddWebSockets(options =>
{
    
});
var app = builder.Build();

app.UseWebSockets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();