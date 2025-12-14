using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Sanity check:
app.MapGet("/", () => "Todo API is running");

/* Test: */
var todos = new List<Todo>
{
    new Todo { Id = 1, Title = "Learn .NET Minimal APIs", IsComplete = false },
    new Todo { Id = 2, Title = "Build a small portfolio project", IsComplete = false }
};

app.MapGet("/todos", () =>
{
    return Results.Ok(todos);
});

app.MapGet("/todos/{id:int}", (int id) =>
{
    var todo = todos.FirstOrDefault(t => t.Id == id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
});

app.MapPost("/todos", (Todo newTodo) =>
{
    if (string.IsNullOrWhiteSpace(newTodo.Title))
        return Results.BadRequest("Title is required.");

    // Generate a new ID
    var newId = todos.Count == 0 ? 1 : todos.Max(t => t.Id) + 1;
    newTodo.Id = newId;

    todos.Add(newTodo);

    return Results.Created($"/todos/{newTodo.Id}", newTodo);
});



/*
 * Endpoints will go here in later issues.
 */

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}