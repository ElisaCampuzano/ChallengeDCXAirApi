using Application.Interfaces;
using Application.Mapper;
using Application.Services;
using Domain.Entities;
using Infrastructure.Repositories;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:4200") 
              .AllowAnyHeader()  
              .AllowAnyMethod();  
    });
});

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IFlightRepository, FlightInMemoryRepository>();
builder.Services.AddAutoMapper(typeof(Mapper));
builder.Services.AddHttpClient<CurrencyConverterService>();
builder.Services.AddScoped<IAirportService, AirportService>();
builder.Services.AddScoped<IAirportRepository, AirportRepository>();

var app = builder.Build();

// Enable CORS
app.UseCors("AllowAngularLocalhost");

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
