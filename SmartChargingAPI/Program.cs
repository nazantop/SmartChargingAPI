using MongoDB.Driver;
using SmartChargingAPI.Helpers.Mappers;
using SmartChargingAPI.IServices;
using SmartChargingAPI.Repositories.Implementation;
using SmartChargingAPI.Repositories.Interfaces;
using SmartChargingAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration["MongoDB:ConnectionString"];
    return new MongoClient(connectionString);
});

builder.Services.AddAutoMapper(typeof(CommonProfile));

builder.Services.AddSingleton<IChargeStationService, ChargeStationService>();
builder.Services.AddSingleton<IGroupService, GroupService>();
builder.Services.AddSingleton<IConnectorService, ConnectorService>();

builder.Services.AddSingleton<IGroupRepository, GroupRepository>();
builder.Services.AddSingleton<IChargeStationRepository, ChargeStationRepository>();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();