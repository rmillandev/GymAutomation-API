using GymAutomationApi.Configuration;
using GymAutomationApi.Interfaces;
using GymAutomationApi.Middleware;
using GymAutomationApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient();

builder.Services.Configure<GeminiConfig>(builder.Configuration.GetSection("GeminiConfig"));
builder.Services.Configure<GoogleConfig>(builder.Configuration.GetSection("GoogleConfig"));

builder.Services.AddScoped<IGeminiService, GeminiService>();
builder.Services.AddScoped<ICalendarService, GoogleCalendarService>();

var app = builder.Build();

app.UseExceptionHandling();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
