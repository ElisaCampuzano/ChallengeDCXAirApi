using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Infrastructure.Repositories;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IFlightService, FlightService>();
builder.Services.AddSingleton<IFlightRepository, FlightInMemoryRepository>();

var app = builder.Build();

// Load JSON data and store in the repository
using (var scope = app.Services.CreateScope())
{
    var flightRepository = scope.ServiceProvider.GetRequiredService<IFlightRepository>();

    var flightJson = File.ReadAllText("markets.json");
    var flights = JsonSerializer.Deserialize<IEnumerable<Flight>>(flightJson, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true 
    });

    if (flights != null)
    {
        await flightRepository.AddFlightsAsync(flights);
    }
}

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
