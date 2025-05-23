using LaWashSmartParking.Application.Interfaces;
using LaWashSmartParking.Application.Services;
using LaWashSmartParking.Infrastructure.Data;
using LaWashSmartParking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SmartParkingDbContext>(options =>
    options.UseSqlite("Data Source=smart_parking.db"));

builder.Services.AddScoped<IParkingSpotRepository, EfParkingSpotRepository>();
builder.Services.AddScoped<IIoTDeviceRepository, EfIoTDeviceRepository>();
builder.Services.AddScoped<IParkingUsageLogRepository, EfParkingUsageLogRepository>();
builder.Services.AddScoped<IIoTDeviceService, IoTDeviceService>();
builder.Services.AddScoped<IParkingSpotService, ParkingSpotService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

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