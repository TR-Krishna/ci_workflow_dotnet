using tester.Repositories;
using tester.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC controllers
builder.Services.AddControllers();

// DI wiring - swap InMemoryMeterReadingRepository for a real DB-backed
// repository later (e.g. EF Core + PostgreSQL) without touching the service
// or controller layers at all. That's the whole point of the interface.
builder.Services.AddSingleton<IMeterReadingRepository, InMemoryMeterReadingRepository>();
builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Exposes the implicit Program class to the test project so
// WebApplicationFactory<Program> can spin up the app in-process for
// integration tests.
public partial class Program { }
