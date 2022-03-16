using Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ITestRunQueue, TestRunQueue>();
builder.Services.AddSingleton<ITestRunEnqueue>(p => p.GetRequiredService<ITestRunQueue>());
builder.Services.AddSingleton<ITestRunDequeue>(p => p.GetRequiredService<ITestRunQueue>());
builder.Services.AddScoped<TestRunner>();

builder.Services.AddTransient<TestRunningBackgroundService>();
builder.Services.AddSingleton<IHostedService>(p => p.GetRequiredService<TestRunningBackgroundService>());
builder.Services.AddSingleton<IHostedService>(p => p.GetRequiredService<TestRunningBackgroundService>());
builder.Services.AddSingleton<IHostedService>(p => p.GetRequiredService<TestRunningBackgroundService>());
//builder.Services.AddHostedService<TestRunningBackgroundService>();

builder.Services.AddOptions<TestRunWorkersOptions>()
    .Bind(builder.Configuration.GetSection(TestRunWorkersOptions.Name));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app
    .MapPost("/runtest", async (ITestRunEnqueue queue) =>
    {
        var id = Guid.NewGuid();
        await queue.RunTestAsync(id);

        return $"Running test {id}";
    })
    .WithName("RunTest");
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}